using AuxiliumSoftware.AuxiliumServices.Common.DataTransferObjects;
using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework;
using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels;
using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.Services.Implementations
{
    public class WebApplicationFirewallService : IWebApplicationFirewallService
    {
        private readonly AuxiliumDbContext _db;
        private readonly ISystemSettingsService _settings;
        private readonly ILogger<WebApplicationFirewallService> _logger;

        private static readonly ConcurrentDictionary<Guid, SemaphoreSlim> _userLocks = new();

        private const string WafLogPrefix = "[WAF]";

        public WebApplicationFirewallService(
            AuxiliumDbContext db,
            ISystemSettingsService settings,
            ILogger<WebApplicationFirewallService> logger
        )
        {
            _db = db;
            _settings = settings;
            _logger = logger;
        }

        #region Utilities
        public string NormaliseIpAddress(IPAddress ipAddress)
        {
            if (ipAddress.IsIPv4MappedToIPv6)
                return ipAddress.MapToIPv4().ToString();

            return ipAddress.ToString();
        }

        /// <summary>
        /// Returns a normalised IPAddress object (mapping IPv4-mapped-IPv6 to plain IPv4).
        /// Use this for comparisons against entity properties that use IpAddressConverter, rather than calling NormaliseIpAddress() inside a LINQ expression tree.
        /// </summary>
        private IPAddress NormaliseIpAddressObject(IPAddress ipAddress)
        {
            if (ipAddress.IsIPv4MappedToIPv6)
                return ipAddress.MapToIPv4();

            return ipAddress;
        }





        private bool IsInCidrRange(IPAddress ipAddress, string cidr)
        {
            try
            {
                string[] parts = cidr.Split('/');
                if (parts.Length != 2)
                {
                    _logger.LogWarning(
                        "{Prefix}: Invalid CIDR format (missing prefix): {Cidr}",
                        WafLogPrefix, cidr
                    );
                    return false;
                }

                if (!IPAddress.TryParse(parts[0], out var networkAddress))
                {
                    _logger.LogWarning(
                        "{Prefix}: Invalid CIDR network address: {Cidr}",
                        WafLogPrefix, cidr
                    );
                    return false;
                }

                if (!int.TryParse(parts[1], out var prefixLength) || prefixLength < 0)
                {
                    _logger.LogWarning(
                        "{Prefix}: Invalid CIDR prefix length: {Cidr}",
                        WafLogPrefix, cidr
                    );
                    return false;
                }

                byte[] ipBytes = ipAddress.GetAddressBytes();
                byte[] networkBytes = networkAddress.GetAddressBytes();

                if (ipBytes.Length != networkBytes.Length)
                    return false;

                int totalBits = ipBytes.Length * 8;
                if (prefixLength > totalBits)
                {
                    _logger.LogWarning(
                        "{Prefix}: CIDR prefix length exceeds address size: {Cidr}",
                        WafLogPrefix, cidr
                    );
                    return false;
                }

                int bitsToCompare = prefixLength;

                for (int i = 0; i < bitsToCompare; i++)
                {
                    int byteIndex = i / 8;
                    int bitIndex = 7 - (i % 8);
                    byte mask = (byte)(1 << bitIndex);

                    if ((ipBytes[byteIndex] & mask) != (networkBytes[byteIndex] & mask))
                        return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "{Prefix}: Error parsing CIDR entry: {Cidr}",
                    WafLogPrefix, cidr
                );
                return false;
            }
        }
        #endregion

        #region Request Validation
        public async Task<SystemWafIpBlacklistEntryEntityModel?> IsIpAddressBlacklistedAsync(IPAddress ipAddress, CancellationToken ct = default)
        {
            bool enabled = await _settings.GetBoolAsync(SystemSettingKeyEnum.Policies_WebApplicationFirewall_Enabled);
            if (!enabled)
                return null;

            if (await IsWhitelistedAsync(ipAddress, ct))
                return null;

            // normalise once on the C# side; the IpAddressConverter handles the
            // DB-side string comparison so EF can translate this to a simple WHERE
            IPAddress normalizedIp = NormaliseIpAddressObject(ipAddress);

            SystemWafIpBlacklistEntryEntityModel? block = await _db.System_Waf_IpBlacklist
                .Where(b => b.IpAddress == normalizedIp)
                .Where(b => b.UnblacklistedAtUtc == null && (b.ExpiresAtUtc == null || b.ExpiresAtUtc > DateTime.UtcNow))
                .OrderBy(b => b.CreatedAtUtc)
                .FirstOrDefaultAsync(ct);

            if (block == null)
                return null;

            return block;
        }

        public async Task<bool> IsIpAddressRateLimitedAsync(IPAddress ipAddress, CancellationToken ct = default)
        {
            if (!await _settings.GetBoolAsync(SystemSettingKeyEnum.Policies_WebApplicationFirewall_Enabled))
                return false;

            if (!await _settings.GetBoolAsync(SystemSettingKeyEnum.Policies_WebApplicationFirewall_RateLimiting_Enabled))
                return false;

            if (await IsWhitelistedAsync(ipAddress, ct))
                return false;

            string normalizedIp = NormaliseIpAddress(ipAddress);
            DateTime windowStart = DateTime.UtcNow.AddMinutes(-1);

            int numberOfRecentAttempts = await _db.Log_LoginAttempts
                .CountAsync(
                    a => a.ClientIpAddress == normalizedIp
                    && a.CreatedAtUtc >= windowStart,
                    ct
                );

            if (numberOfRecentAttempts < await _settings.GetIntAsync(SystemSettingKeyEnum.Policies_WebApplicationFirewall_RateLimiting_MaximumLoginRequestsPerMinute))
                return false;

            return true;
        }

        public async Task<SystemWafUserBlacklistEntryEntityModel?> IsUserBlacklistedAsync(UserEntityModel user, CancellationToken ct = default)
        {
            if (!await _settings.GetBoolAsync(SystemSettingKeyEnum.Policies_WebApplicationFirewall_Enabled))
                return null;

            DateTime now = DateTime.UtcNow;

            return await _db.System_Waf_UserBlacklist
                .Where(b => b.UserId == user.Id)
                .Where(b => b.UnblacklistedAtUtc == null)
                .Where(b => (b.ExpiresAtUtc == null || b.ExpiresAtUtc > now))
                .OrderByDescending(b => b.CreatedAtUtc)
                .FirstOrDefaultAsync(ct);
        }

        public async Task<SystemWafUserBlacklistEntryEntityModel?> ApplyLockoutIfNeededAsync(UserEntityModel user, CancellationToken ct = default)
        {
            if (!await _settings.GetBoolAsync(SystemSettingKeyEnum.Policies_WebApplicationFirewall_Enabled))
                return null;

            if (await _settings.GetIntAsync(SystemSettingKeyEnum.Policies_WebApplicationFirewall_User_MaximumFailedLoginsPerUser) == 0)
                return null;

            SemaphoreSlim userSemaphore = _userLocks.GetOrAdd(user.Id, _ => new SemaphoreSlim(1, 1));

            await userSemaphore.WaitAsync(ct);
            try
            {
                DateTime now = DateTime.UtcNow;
                SystemWafUserBlacklistEntryEntityModel? existingLock = await _db.System_Waf_UserBlacklist
                    .Where(b => b.UserId == user.Id)
                    .Where(b => b.UnblacklistedAtUtc == null && (b.ExpiresAtUtc == null || b.ExpiresAtUtc > now))
                    .FirstOrDefaultAsync(ct);

                if (existingLock != null)
                {
                    return existingLock;
                }

                DateTime windowStart = now.AddMinutes(-await _settings.GetIntAsync(SystemSettingKeyEnum.Policies_WebApplicationFirewall_User_UserLockoutWindowInMinutes));

                int failedCount = await _db.Log_LoginAttempts
                    .CountAsync(
                        a => a.TargetUserId == user.Id
                        && !a.WasLoginSuccessful
                        && a.CreatedAtUtc >= windowStart,
                        ct
                    );

                if (failedCount < await _settings.GetIntAsync(SystemSettingKeyEnum.Policies_WebApplicationFirewall_User_MaximumFailedLoginsPerUser))
                    return null;

                bool recheck = await _db.System_Waf_UserBlacklist
                    .AnyAsync(
                        b => b.UserId == user.Id
                        && b.UnblacklistedAtUtc == null
                        && (b.ExpiresAtUtc == null || b.ExpiresAtUtc > now),
                        ct
                    );

                if (recheck)
                    return await IsUserBlacklistedAsync(user, ct);

                int lockoutDuration = await _settings.GetIntAsync(SystemSettingKeyEnum.Policies_WebApplicationFirewall_User_UserLockoutDurationInMinutes);
                SystemWafUserBlacklistEntryEntityModel lockout = new()
                {
                    Id = Guid.NewGuid(),
                    CreatedAtUtc = now,
                    UserId = user.Id,
                    JustificationForBlacklist = $"{WafLogPrefix}: Automatic lockout: {failedCount} failed login attempts in {await _settings.GetIntAsync(SystemSettingKeyEnum.Policies_WebApplicationFirewall_User_UserLockoutWindowInMinutes)} minutes",
                    IsPermanent = lockoutDuration == 0,
                    ExpiresAtUtc = lockoutDuration > 0 ? now.AddMinutes(lockoutDuration) : (DateTime?)null
                };

                _db.System_Waf_UserBlacklist.Add(lockout);
                await _db.SaveChangesAsync(ct);

                _logger.LogWarning(
                    "{Prefix}: User {UserId} locked out after {Count} failed attempts. Duration: {Duration}",
                    WafLogPrefix, user.Id, failedCount, lockoutDuration > 0 ? $"{lockoutDuration} minutes" : "permanent"
                );

                return lockout;
            }
            finally
            {
                userSemaphore.Release();
            }
        }
        #endregion

        #region Login Attempt Logging
        public async Task<WebApplicationFirewallActionDTO> RecordFailedLoginAsync(
            IPAddress ipAddress,
            string attemptedEmail,
            UserEntityModel? user,
            LoginAttemptFailureReasonEnum failureReason,
            CancellationToken ct = default
        )
        {
            if (failureReason == LoginAttemptFailureReasonEnum.UserNotFound)
            {
                if (!await _settings.GetBoolAsync(SystemSettingKeyEnum.Policies_Logging_Security_LogIncorrectEmailAddressLoginAttempts))
                    return new WebApplicationFirewallActionDTO();
            }
            else if (failureReason == LoginAttemptFailureReasonEnum.InvalidPassword)
            {
                if (!await _settings.GetBoolAsync(SystemSettingKeyEnum.Policies_Logging_Security_LogIncorrectPasswordLoginAttempts))
                    return new WebApplicationFirewallActionDTO();
            }

            _db.Log_LoginAttempts.Add(new LogLoginAttemptEventEntityModel
            {
                Id = Guid.NewGuid(),
                CreatedAtUtc = DateTime.UtcNow,
                ClientIpAddress = NormaliseIpAddress(ipAddress),
                AttemptedEmailAddress = attemptedEmail,
                TargetUserId = user?.Id,
                WasLoginSuccessful = false,
                WasBlockedByWaf = false,
                FailureReason = failureReason,
            });

            await _db.SaveChangesAsync(ct);

            WebApplicationFirewallActionDTO action = new();

            var enabled = await _settings.GetBoolAsync(SystemSettingKeyEnum.Policies_WebApplicationFirewall_Enabled);
            if (!enabled || await IsWhitelistedAsync(ipAddress, ct))
                return action;

            if (user != null)
            {
                SystemWafUserBlacklistEntryEntityModel? lockout = await ApplyLockoutIfNeededAsync(user, ct);
                if (lockout != null)
                {
                    action.IsUserLocked = true;
                    action.Message = lockout.JustificationForBlacklist;
                }
            }

            await CheckAndApplyIpAddressBlacklistEntryAsync(ipAddress, action, ct);

            if (user != null)
            {
                await CheckDistributedAttackAsync(user, action, ct);
            }

            return action;
        }

        public async Task RecordSuccessfulLoginAsync(
            IPAddress ipAddress,
            string email,
            UserEntityModel user,
            CancellationToken ct = default
        )
        {
            string normalizedIp = NormaliseIpAddress(ipAddress);

            if (await _settings.GetBoolAsync(SystemSettingKeyEnum.Policies_Logging_Security_LogSuccessfulLogins))
            {
                _db.Log_LoginAttempts.Add(new LogLoginAttemptEventEntityModel
                {
                    Id = Guid.NewGuid(),
                    CreatedAtUtc = DateTime.UtcNow,
                    ClientIpAddress = normalizedIp,
                    AttemptedEmailAddress = email,
                    TargetUserId = user.Id,
                    WasLoginSuccessful = true,
                    WasBlockedByWaf = false,
                });
            }

            await _db.SaveChangesAsync(ct);

            _logger.LogInformation(
                "{Prefix}: Successful login for user {UserId} from {IpAddress}",
                WafLogPrefix, user.Id, normalizedIp
            );
        }

        public async Task<int> GetFailureDelayAsync(IPAddress ipAddress, CancellationToken ct = default)
        {
            if (!await _settings.GetBoolAsync(SystemSettingKeyEnum.Policies_WebApplicationFirewall_Enabled))
                return 0;

            if (!await _settings.GetBoolAsync(SystemSettingKeyEnum.Policies_WebApplicationFirewall_RateLimiting_Enabled))
                return 0;

            int baseDelay = await _settings.GetIntAsync(SystemSettingKeyEnum.Policies_WebApplicationFirewall_RateLimiting_FailureDelayBaseMilliseconds);
            int maxDelay = await _settings.GetIntAsync(SystemSettingKeyEnum.Policies_WebApplicationFirewall_RateLimiting_FailureDelayMaximumMilliseconds);

            if (baseDelay == 0)
                return 0;

            string normalizedIp = NormaliseIpAddress(ipAddress);
            int ipBlockWindow = await _settings.GetIntAsync(SystemSettingKeyEnum.Policies_WebApplicationFirewall_Ip_IpBlacklistWindowInMinutes);
            int recentFailures = await _db.Log_LoginAttempts
                .CountAsync(
                    a => a.ClientIpAddress == normalizedIp
                    && !a.WasLoginSuccessful
                    && a.CreatedAtUtc >= DateTime.UtcNow.AddMinutes(-ipBlockWindow),
                    ct
                );

            if (recentFailures == 0)
                return 0;

            int exponent = Math.Min(recentFailures - 1, 30);
            long delay = (long)baseDelay * (1L << exponent);
            return (int)Math.Min(delay, maxDelay);
        }
        #endregion

        #region Internal Helper Methods
        private async Task CheckAndApplyIpAddressBlacklistEntryAsync(
            IPAddress ipAddress,
            WebApplicationFirewallActionDTO action,
            CancellationToken ct
        )
        {
            string normalizedIp = NormaliseIpAddress(ipAddress);
            IPAddress normalizedIpObj = NormaliseIpAddressObject(ipAddress);
            DateTime now = DateTime.UtcNow;
            DateTime windowStart = now.AddMinutes(-(await _settings.GetIntAsync(SystemSettingKeyEnum.Policies_WebApplicationFirewall_Ip_IpBlacklistWindowInMinutes)));

            int recentFailures = await _db.Log_LoginAttempts
                .CountAsync(
                    a => a.ClientIpAddress == normalizedIp
                    && !a.WasLoginSuccessful
                    && a.CreatedAtUtc >= windowStart,
                    ct
                );

            if (recentFailures < await _settings.GetIntAsync(SystemSettingKeyEnum.Policies_WebApplicationFirewall_Ip_MaximumFailedLoginsPerIp))
                return;

            int numberOfTemporaryBlocksBeforeAPermanentBan = await _settings.GetIntAsync(SystemSettingKeyEnum.Policies_WebApplicationFirewall_Ip_TemporaryBlacklistsBeforePermanentBlacklist);
            int permananetBandWindow = await _settings.GetIntAsync(SystemSettingKeyEnum.Policies_WebApplicationFirewall_Ip_PermanentBanWindowHours);
            int temporaryBlockDuration = await _settings.GetIntAsync(SystemSettingKeyEnum.Policies_WebApplicationFirewall_Listing_TemporaryIpBlacklistDurationInMinutes_Default);

            List<SystemWafIpBlacklistEntryEntityModel>? blockHistory = await _db.System_Waf_IpBlacklist
                .Where(b => b.IpAddress == normalizedIpObj)
                .ToListAsync(ct);

            int previousBlockCount = blockHistory.Count;
            int recentTempBlocks = blockHistory.Count(
                b => !b.IsPermanent
                && b.CreatedAtUtc >= now.AddHours(-permananetBandWindow)
            );
            List<Guid> activeBlockIds = blockHistory
                .Where(b => b.UnblacklistedAtUtc == null && (b.ExpiresAtUtc == null || b.ExpiresAtUtc > now))
                .Select(b => b.Id)
                .ToList();

            if (activeBlockIds.Count > 0)
            {
                await _db.System_Waf_IpBlacklist
                    .Where(b => activeBlockIds.Contains(b.Id))
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(b => b.UnblacklistedAtUtc, now)
                        .SetProperty(b => b.JustificationForUnblacklist, $"Superseded by new block"), ct);
            }

            bool shouldBePermanent =
                previousBlockCount + 1 >= numberOfTemporaryBlocksBeforeAPermanentBan
                && recentTempBlocks >= numberOfTemporaryBlocksBeforeAPermanentBan - 1;

            if (shouldBePermanent)
            {
                _db.System_Waf_IpBlacklist.Add(new SystemWafIpBlacklistEntryEntityModel
                {
                    Id = Guid.NewGuid(),
                    CreatedAtUtc = now,
                    IpAddress = ipAddress,
                    IsPermanent = true,
                    JustificationForBlacklist = $"{WafLogPrefix}: Automatic permanent ban: {previousBlockCount + 1} blocks within {permananetBandWindow} hours"
                });

                await _db.SaveChangesAsync(ct);

                action.IsIpPermanentlyBlocked = true;
                action.Message = $"{WafLogPrefix}: IP permanently banned due to sustained attack";

                _logger.LogCritical(
                    "{Prefix}: IP {IpAddress} PERMANENTLY BANNED after {Count} blocks",
                    WafLogPrefix, normalizedIp, previousBlockCount + 1
                );

                return;
            }
            else
            {
                _db.System_Waf_IpBlacklist.Add(new SystemWafIpBlacklistEntryEntityModel
                {
                    Id = Guid.NewGuid(),
                    CreatedAtUtc = now,
                    IpAddress = ipAddress,
                    IsPermanent = false,
                    JustificationForBlacklist = $"{WafLogPrefix}: Automatic temp block: {recentFailures} failed logins in {await _settings.GetIntAsync(SystemSettingKeyEnum.Policies_WebApplicationFirewall_Ip_IpBlacklistWindowInMinutes)} minutes",
                    ExpiresAtUtc = now.AddMinutes(temporaryBlockDuration)
                });

                await _db.SaveChangesAsync(ct);

                action.IsIpTemporarilyBlocked = true;
                action.Message = $"IP temporarily blocked for {temporaryBlockDuration} minutes";

                _logger.LogWarning(
                    "{Prefix}: IP {IpAddress} temporarily blocked (block #{Count}) for {Duration} minutes",
                    WafLogPrefix, normalizedIp, previousBlockCount + 1, temporaryBlockDuration
                );
            }
        }

        private async Task CheckDistributedAttackAsync(
            UserEntityModel user,
            WebApplicationFirewallActionDTO action,
            CancellationToken ct
        )
        {
            if (!await _settings.GetBoolAsync(SystemSettingKeyEnum.Policies_WebApplicationFirewall_Enabled))
                return;

            if (!await _settings.GetBoolAsync(SystemSettingKeyEnum.Policies_WebApplicationFirewall_DistributedAttackDetection_Enabled))
                return;

            int windowMinutes = await _settings.GetIntAsync(SystemSettingKeyEnum.Policies_WebApplicationFirewall_DistributedAttackDetection_WindowInMinutes);

            if (await _settings.GetIntAsync(SystemSettingKeyEnum.Policies_WebApplicationFirewall_DistributedAttackDetection_FailedLoginsFromDistinctIpsThreshold) == 0)
                return;

            DateTime now = DateTime.UtcNow;

            int distinctIps = await _db.Log_LoginAttempts
                .Where(
                    a => a.TargetUserId == user.Id
                    && !a.WasLoginSuccessful
                    && a.CreatedAtUtc >= now.AddMinutes(-windowMinutes)
                )
                .Select(a => a.ClientIpAddress)
                .Distinct()
                .CountAsync(ct);

            if (distinctIps < await _settings.GetIntAsync(SystemSettingKeyEnum.Policies_WebApplicationFirewall_DistributedAttackDetection_FailedLoginsFromDistinctIpsThreshold))
                return;

            action.IsDistributedAttackDetected = true;

            _logger.LogCritical(
                "{Prefix}: Distributed attack detected! User {UserId} targeted from {Count} distinct IPs in {Window} minutes",
                WafLogPrefix, user.Id, distinctIps, windowMinutes
            );

            bool isUserAlreadyLocked = await _db.System_Waf_UserBlacklist
                .AnyAsync(
                    b => b.UserId == user.Id
                    && b.UnblacklistedAtUtc == null
                    && (b.ExpiresAtUtc == null || b.ExpiresAtUtc > now),
                    ct
                );

            if (isUserAlreadyLocked)
                return;

            _db.System_Waf_UserBlacklist.Add(new SystemWafUserBlacklistEntryEntityModel
            {
                Id = Guid.NewGuid(),
                CreatedAtUtc = now,
                UserId = user.Id,
                JustificationForBlacklist = $"{WafLogPrefix}: Distributed attack detected: {distinctIps} distinct IPs in {windowMinutes} minutes",
                IsPermanent = true,
                ExpiresAtUtc = null
            });

            await _db.SaveChangesAsync(ct);

            action.IsUserLocked = true;
            action.Message = $"{WafLogPrefix}: Account locked due to distributed attack detection - contact administrator";
        }
        #endregion

        #region Blacklist Operations
        public async Task BlacklistIpAddressAsync(
            IPAddress ipAddress,
            string reason,
            UserEntityModel adminUser,
            bool permanent = true,
            CancellationToken ct = default
        )
        {
            var now = DateTime.UtcNow;
            IPAddress normalizedIpObj = NormaliseIpAddressObject(ipAddress);

            await _db.System_Waf_IpBlacklist
                .Where(
                    b => b.IpAddress == normalizedIpObj
                    && b.UnblacklistedAtUtc == null
                    && (b.ExpiresAtUtc == null || b.ExpiresAtUtc > now)
                )
                .ExecuteUpdateAsync(s => s
                    .SetProperty(b => b.UnblacklistedAtUtc, now)
                    .SetProperty(b => b.JustificationForUnblacklist, $"{WafLogPrefix}: Superseded by manual block"), ct);

            int tempBlockDuration = await _settings.GetIntAsync(SystemSettingKeyEnum.Policies_WebApplicationFirewall_Listing_TemporaryIpBlacklistDurationInMinutes_Default);

            _db.System_Waf_IpBlacklist.Add(new SystemWafIpBlacklistEntryEntityModel
            {
                Id = Guid.NewGuid(),
                CreatedAtUtc = now,
                CreatedByUserId = adminUser.Id,
                IpAddress = ipAddress,
                IsPermanent = permanent,
                JustificationForBlacklist = reason,
                ExpiresAtUtc = permanent ? null : now.AddMinutes(tempBlockDuration)
            });

            await _db.SaveChangesAsync(ct);

            _logger.LogWarning(
                "{Prefix}: IP `{IpAddress}` manually blacklisted by admin {AdminId}. Reason: {Reason}. Permanent: {Permanent}",
                WafLogPrefix, NormaliseIpAddress(ipAddress), adminUser.Id, reason.Replace("\n", "").Replace("\r", "").Trim(), permanent
            );
        }

        public async Task<bool> RemoveIpAddressFromBlacklistAsync(
            IPAddress ipAddress,
            UserEntityModel adminUser,
            CancellationToken ct = default
        )
        {
            DateTime now = DateTime.UtcNow;
            IPAddress normalizedIpObj = NormaliseIpAddressObject(ipAddress);

            int affected = await _db.System_Waf_IpBlacklist
                .Where(
                    b => b.IpAddress == normalizedIpObj
                    && b.UnblacklistedAtUtc == null
                    && (
                        b.ExpiresAtUtc == null
                        || b.ExpiresAtUtc > now
                    )
                )
                .ExecuteUpdateAsync(s => s
                    .SetProperty(b => b.UnblacklistedAtUtc, now)
                    .SetProperty(b => b.UnblacklistedBy, adminUser.Id)
                    .SetProperty(b => b.JustificationForUnblacklist, "[Manual]: Manually removed from blacklist by administrator"), ct);

            if (affected == 0)
                return false;

            _logger.LogInformation(
                "{Prefix}: IP {IpAddress} removed from blacklist by admin {AdminId}",
                WafLogPrefix, NormaliseIpAddress(ipAddress), adminUser.Id
            );

            return true;
        }

        public async Task BlacklistUserAsync(
            UserEntityModel user,
            string reason,
            UserEntityModel adminUser,
            bool permanent = true,
            CancellationToken ct = default
        )
        {
            DateTime now = DateTime.UtcNow;

            // mark any existing active blocks as superseded
            await _db.System_Waf_UserBlacklist
                .Where(
                    b => b.UserId == user.Id
                    && b.UnblacklistedAtUtc == null
                    && (b.ExpiresAtUtc == null || b.ExpiresAtUtc > now)
                )
                .ExecuteUpdateAsync(s => s
                    .SetProperty(b => b.UnblacklistedAtUtc, now)
                    .SetProperty(b => b.JustificationForUnblacklist, $"{WafLogPrefix}: Superseded by manual block"), ct);

            int lockoutDuration = await _settings.GetIntAsync(SystemSettingKeyEnum.Policies_WebApplicationFirewall_User_UserLockoutDurationInMinutes);

            _db.System_Waf_UserBlacklist.Add(new SystemWafUserBlacklistEntryEntityModel
            {
                Id = Guid.NewGuid(),
                CreatedAtUtc = now,
                CreatedByUserId = adminUser.Id,
                UserId = user.Id,
                IsPermanent = permanent,
                JustificationForBlacklist = reason,
                ExpiresAtUtc = permanent ? null : now.AddMinutes(lockoutDuration)
            });

            await _db.SaveChangesAsync(ct);

            _logger.LogWarning(
                "{Prefix}: User {UserId} manually blacklisted by admin {AdminId}. Reason: {Reason}. Permanent: {Permanent}",
                WafLogPrefix, user.Id, adminUser.Id, reason.Replace("\n", "").Replace("\r", "").Trim(), permanent
            );
        }

        public async Task<bool> RemoveUserFromBlacklist(
            UserEntityModel user,
            UserEntityModel adminUser,
            CancellationToken ct = default
        )
        {
            DateTime now = DateTime.UtcNow;

            int affected = await _db.System_Waf_UserBlacklist
                .Where(
                    b => b.UserId == user.Id
                    && b.UnblacklistedAtUtc == null
                    && (
                        b.ExpiresAtUtc == null
                        || b.ExpiresAtUtc > now
                    )
                )
                .ExecuteUpdateAsync(s => s
                    .SetProperty(b => b.UnblacklistedAtUtc, now)
                    .SetProperty(b => b.UnblacklistedBy, adminUser.Id)
                    .SetProperty(b => b.JustificationForUnblacklist, "[Manual]: Manually removed from blacklist by administrator"), ct);

            if (affected == 0)
                return false;

            _logger.LogInformation(
                "{Prefix}: User {UserId} unlocked by admin {AdminId}",
                WafLogPrefix, user.Id, adminUser.Id
            );

            return true;
        }
        #endregion

        #region Whitelist Operations
        public async Task WhitelistIpAddressAsync(
            IPAddress ipAddress,
            string reason,
            UserEntityModel adminUser,
            bool permanent = true,
            CancellationToken ct = default
        )
        {
            DateTime now = DateTime.UtcNow;

            // mark any existing active whitelist entries as superseded
            await _db.System_Waf_IpWhitelist
                .Where(
                    w => w.IpAddress == ipAddress
                    && w.UnwhitelistedAtUtc == null
                    && (w.ExpiresAtUtc == null || w.ExpiresAtUtc > now)
                )
                .ExecuteUpdateAsync(s => s
                    .SetProperty(w => w.UnwhitelistedAtUtc, now)
                    .SetProperty(w => w.JustificationForUnwhitelist, $"{WafLogPrefix}: Superseded by new whitelist entry"),
                ct
            );

            int whitelistDuration = await _settings.GetIntAsync(SystemSettingKeyEnum.Policies_WebApplicationFirewall_Listing_TemporaryIpWhitelistDurationInMinutes_Default);

            _db.System_Waf_IpWhitelist.Add(new SystemWafIpWhitelistEntryEntityModel
            {
                Id = Guid.NewGuid(),
                CreatedAtUtc = now,
                CreatedByUserId = adminUser.Id,
                IpAddress = ipAddress,
                IsPermanent = permanent,
                JustificationForWhitelist = reason,
                ExpiresAtUtc = permanent ? null : now.AddMinutes(whitelistDuration)
            });

            await _db.SaveChangesAsync(ct);

            _logger.LogInformation(
                "{Prefix}: IP {IpAddress} whitelisted by admin {AdminId}. Reason: {Reason}. Permanent: {Permanent}",
                WafLogPrefix, ipAddress, adminUser.Id, reason.Replace("\n", "").Replace("\r", "").Trim(), permanent
            );
        }

        public async Task<bool> RemoveIpAddressFromWhitelistAsync(
            IPAddress ipAddress,
            UserEntityModel adminUser,
            CancellationToken ct = default
        )
        {
            DateTime now = DateTime.UtcNow;
            string normalizedIp = NormaliseIpAddress(ipAddress);

            int affected = await _db.System_Waf_IpWhitelist
                .Where(
                    w => w.IpAddress == ipAddress
                    && w.UnwhitelistedAtUtc == null
                    && (w.ExpiresAtUtc == null || w.ExpiresAtUtc > now)
                )
                .ExecuteUpdateAsync(s => s
                    .SetProperty(w => w.UnwhitelistedAtUtc, now)
                    .SetProperty(w => w.UnwhitelistedBy, adminUser.Id)
                    .SetProperty(w => w.JustificationForUnwhitelist, "[Manual]: Manually removed from whitelist by administrator"), ct);

            if (affected == 0)
                return false;

            _logger.LogInformation(
                "{Prefix}: IP {IpAddress} removed from whitelist by admin {AdminId}",
                WafLogPrefix, normalizedIp, adminUser.Id
            );

            return true;
        }

        public async Task WhitelistUserAsync(
            UserEntityModel user,
            string reason,
            UserEntityModel adminUser,
            bool permanent = true,
            CancellationToken ct = default
        )
        {
            DateTime now = DateTime.UtcNow;

            // mark any existing active whitelist entries as superseded
            await _db.System_Waf_UserWhitelist
                .Where(
                    w => w.UserId == user.Id
                    && w.UnwhitelistedAtUtc == null
                    && (w.ExpiresAtUtc == null || w.ExpiresAtUtc > now)
                )
                .ExecuteUpdateAsync(s => s
                    .SetProperty(w => w.UnwhitelistedAtUtc, now)
                    .SetProperty(w => w.JustificationForUnwhitelist, $"{WafLogPrefix}: Superseded by new whitelist entry"), ct);

            int whitelistDuration = await _settings.GetIntAsync(SystemSettingKeyEnum.Policies_WebApplicationFirewall_Listing_TemporaryUserWhitelistDurationInMinutes_Default);

            _db.System_Waf_UserWhitelist.Add(new SystemWafUserWhitelistEntryEntityModel
            {
                Id = Guid.NewGuid(),
                CreatedAtUtc = now,
                CreatedByUserId = adminUser.Id,
                UserId = user.Id,
                IsPermanent = permanent,
                JustificationForWhitelist = reason,
                ExpiresAtUtc = permanent ? null : now.AddMinutes(whitelistDuration)
            });

            await _db.SaveChangesAsync(ct);

            _logger.LogInformation(
                "{Prefix}: User {UserId} whitelisted by admin {AdminId}. Reason: {Reason}. Permanent: {Permanent}",
                WafLogPrefix, user.Id, adminUser.Id, reason.Replace("\n", "").Replace("\r", "").Trim(), permanent
            );
        }

        public async Task<bool> RemoveUserFromWhitelistAsync(
            UserEntityModel user,
            UserEntityModel adminUser,
            CancellationToken ct = default
        )
        {
            DateTime now = DateTime.UtcNow;

            int affected = await _db.System_Waf_UserWhitelist
                .Where(
                    w => w.UserId == user.Id
                    && w.UnwhitelistedAtUtc == null
                    && (w.ExpiresAtUtc == null || w.ExpiresAtUtc > now)
                )
                .ExecuteUpdateAsync(s => s
                    .SetProperty(w => w.UnwhitelistedAtUtc, now)
                    .SetProperty(w => w.UnwhitelistedBy, adminUser.Id)
                    .SetProperty(w => w.JustificationForUnwhitelist, "[Manual]: Manually removed from whitelist by administrator"), ct);

            if (affected == 0)
                return false;

            _logger.LogInformation(
                "{Prefix}: User {UserId} removed from whitelist by admin {AdminId}",
                WafLogPrefix, user.Id, adminUser.Id
            );

            return true;
        }
        #endregion

        #region General Blacklist/Whitelist Operations
        public async Task<List<SystemWafIpBlacklistEntryEntityModel>> GetBlacklistedIpAddressesAsync(
            bool includeExpired = false,
            CancellationToken ct = default
        )
        {
            DateTime now = DateTime.UtcNow;
            var query = _db.System_Waf_IpBlacklist.AsQueryable();

            if (!includeExpired)
                query = query.Where(
                    b => b.UnblacklistedAtUtc == null
                    && (
                        b.ExpiresAtUtc == null
                        || b.ExpiresAtUtc > now
                    )
                );

            return await query
                .OrderByDescending(b => b.CreatedAtUtc)
                .ToListAsync(ct);
        }

        public async Task<bool> IsWhitelistedAsync(IPAddress ipAddress, CancellationToken ct = default)
        {
            DateTime now = DateTime.UtcNow;

            // query the whitelist table for active entries
            List<SystemWafIpWhitelistEntryEntityModel> whitelistedIps = await _db.System_Waf_IpWhitelist
                .Where(
                    w => w.UnwhitelistedAtUtc == null
                    && (w.ExpiresAtUtc == null || w.ExpiresAtUtc > now)
                )
                .ToListAsync(ct);

            if (whitelistedIps == null || whitelistedIps.Count == 0)
                return false;

            string normalizedIp = NormaliseIpAddress(ipAddress);

            foreach (SystemWafIpWhitelistEntryEntityModel entry in whitelistedIps)
            {
                if (string.IsNullOrWhiteSpace(entry.IpAddress.ToString()))
                    continue;

                // check if it's a CIDR range
                if (entry.IpAddress.ToString().Contains('/'))
                {
                    if (IsInCidrRange(ipAddress, entry.IpAddress.ToString()))
                        return true;
                }
                else
                {
                    // direct IP comparison
                    if (IPAddress.TryParse(entry.IpAddress.ToString(), out var whitelistIp))
                    {
                        if (NormaliseIpAddress(whitelistIp) == normalizedIp)
                            return true;
                    }
                }
            }

            return false;
        }
        #endregion

        #region Admin Operations
        public async Task<List<LogLoginAttemptEventEntityModel>> GetRecentLoginAttemptsAsync(
            int count = 100,
            string? ipFilter = null,
            Guid? userIdFilter = null,
            bool? successFilter = null,
            CancellationToken ct = default
        )
        {
            var query = _db.Log_LoginAttempts.AsQueryable();

            if (!string.IsNullOrEmpty(ipFilter))
                query = query.Where(a => a.ClientIpAddress == ipFilter);

            if (userIdFilter.HasValue)
                query = query.Where(a => a.TargetUserId == userIdFilter);

            if (successFilter.HasValue)
                query = query.Where(a => a.WasLoginSuccessful == successFilter);

            return await query
                .OrderByDescending(a => a.CreatedAtUtc)
                .Take(count)
                .ToListAsync(ct);
        }
        #endregion
    }
}
