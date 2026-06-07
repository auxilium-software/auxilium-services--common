using AuxiliumSoftware.AuxiliumServices.Common.Configuration;
using AuxiliumSoftware.AuxiliumServices.Common.DataTransferObjects;
using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework;
using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels;
using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OtpNet;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.Services.Implementations
{
    public class TotpService : ITotpService
    {
        private readonly AuxiliumDbContext _db;
        private readonly SystemSettingsService _systemSettings;
        private readonly ILogger<TotpService> _logger;

        // rfc 6238 defaults
        // these are largely dictated by common authenticator apps (google authenticator, authy, etc.)
        // and therefore should not be changed as many of those apps silently ignore non-standard values
        private const int TimeStepSeconds = 30;
        private const int CodeDigits = 6;
        private const int ToleranceSteps = 1; // +-1 step = +-30s clock drift

        public TotpService(
            AuxiliumDbContext db,
            ISystemSettingsService systemSettingsService,
            ILogger<TotpService> logger
        )
        {
            _systemSettings = new SystemSettingsService(db);

            _db = db;
            _logger = logger;
        }

        #region Enrolment Lifecycle
        public async Task<TotpSetupResultDTO> CreateSetupAsync(Guid userId, string userEmail)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) throw new InvalidOperationException($"User {userId} not found");

            if (user.TotpEnabled)
            {
                throw new InvalidOperationException("TOTP is already enabled. Disable it first to re-enrol.");
            }

            var secretBytes = KeyGeneration.GenerateRandomKey(20);
            var secret = Base32Encoding.ToString(secretBytes);
            var provisioningUri = await GetProvisioningUri(secret, userEmail);

            // store secret as pending
            // (TotpEnabled remains false until the user proves they've set up their authenticator app by submitting a valid code)
            user.TotpSecret = secret;
            user.TotpEnabled = false;
            user.TotpEnabledAtUtc = null;

            await _db.SaveChangesAsync();

            _logger.LogInformation("TOTP setup created for user {UserId}", userId);

            return new TotpSetupResultDTO
            {
                Secret = secret,
                ProvisioningUri = provisioningUri
            };
        }

        public async Task<TotpEnableResultDTO?> EnablePendingAsync(Guid userId, string code)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return null;

            // must have a pending secret that isn't yet enabled
            if (string.IsNullOrEmpty(user.TotpSecret) || user.TotpEnabled)
                return null;

            if (!VerifyCode(user.TotpSecret, code))
                return null;

            user.TotpEnabled = true;
            user.TotpEnabledAtUtc = DateTime.UtcNow;

            var plaintextCodes = await GenerateRecoveryCodesForUser(userId);

            await _db.SaveChangesAsync();

            _logger.LogInformation(
                "TOTP enabled for user {UserId} with {Count} recovery codes",
                userId, plaintextCodes.Count
            );

            return new TotpEnableResultDTO { RecoveryCodes = plaintextCodes };
        }

        public async Task<bool> DisableAsync(Guid userId, string code)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return false;

            if (!user.TotpEnabled || string.IsNullOrEmpty(user.TotpSecret))
                return false;

            if (!VerifyCode(user.TotpSecret, code))
                return false;

            user.TotpSecret = null;
            user.TotpEnabled = false;
            user.TotpEnabledAtUtc = null;

            var recoveryCodes = await _db.UserTotpRecoveryCodes
                .Where(r => r.CreatedBy == userId)
                .ToListAsync();

            if (recoveryCodes.Count != 0)
            {
                _db.UserTotpRecoveryCodes.RemoveRange(recoveryCodes);
            }

            await _db.SaveChangesAsync();

            _logger.LogWarning("TOTP disabled for user {UserId}, recovery codes cleared", userId);
            return true;
        }
        #endregion
        #region Verification
        public async Task<bool> ValidateUserTotpAsync(Guid userId, string? code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return false;

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return false;

            if (!user.TotpEnabled || string.IsNullOrEmpty(user.TotpSecret))
                return false;

            var isValid = VerifyCode(user.TotpSecret, code);

            if (!isValid)
            {
                _logger.LogWarning("Invalid TOTP code attempt for user {UserId}", userId);
            }

            return isValid;
        }

        public async Task<bool> ValidateRecoveryCodeAsync(Guid userId, string? code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return false;

            var hash = HashRecoveryCode(code);

            var match = await _db.UserTotpRecoveryCodes
                .FirstOrDefaultAsync(r =>
                    r.CreatedBy == userId &&
                    r.CodeHash == hash &&
                    !r.IsUsed
                );

            if (match == null)
            {
                _logger.LogWarning("Invalid recovery code attempt for user {UserId}", userId);
                return false;
            }

            // mark the code as used so that it can't be reused
            match.IsUsed = true;
            match.UsedAtUtc = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            var remaining = await GetRemainingRecoveryCodeCountAsync(userId);

            _logger.LogWarning(
                "Recovery code consumed for user {UserId}. {Remaining} codes remaining",
                userId, remaining
            );

            return true;
        }

        public async Task<List<string>?> RegenerateRecoveryCodesAsync(Guid userId, string totpCode)
        {
            var isValid = await ValidateUserTotpAsync(userId, totpCode);
            if (!isValid) return null;

            var existing = await _db.UserTotpRecoveryCodes
                .Where(r => r.CreatedBy == userId)
                .ToListAsync();

            if (existing.Count != 0)
            {
                _db.UserTotpRecoveryCodes.RemoveRange(existing);
            }

            var plaintextCodes = await GenerateRecoveryCodesForUser(userId);

            await _db.SaveChangesAsync();

            _logger.LogInformation(
                "Recovery codes regenerated for user {UserId}. {Count} new codes issued",
                userId, plaintextCodes.Count
            );

            return plaintextCodes;
        }

        public async Task<int> GetRemainingRecoveryCodeCountAsync(Guid userId)
        {
            return await _db.UserTotpRecoveryCodes
                .CountAsync(r => r.CreatedBy == userId && !r.IsUsed);
        }
        #endregion
        #region Status Queries
        public async Task<bool> IsTotpEnabledAsync(Guid userId)
        {
            return await _db.Users
                .AnyAsync(u => u.Id == userId && u.TotpEnabled);
        }
        public async Task<bool> HasPendingSetupAsync(Guid userId)
        {
            return await _db.Users
                .AnyAsync(u => u.Id == userId && u.TotpSecret != null && !u.TotpEnabled);
        }
        #endregion
        #region Recovery Code Generation
        private async Task<List<string>> GenerateRecoveryCodesForUser(Guid userId)
        {
            var plaintextCodes = new List<string>();
            var now = DateTime.UtcNow;

            for (int i = 0; i < await _systemSettings.GetIntAsync(SystemSettingKeyEnum.Policies_Totp_RecoveryCodeFormatting_NumberOfGroupsInARecoveryCode); i++)
            {
                var raw = await GenerateRecoveryCode();
                var formatted = await FormatRecoveryCode(raw);
                plaintextCodes.Add(formatted);

                // hash the formatted code - what the user sees is what they must enter
                _db.UserTotpRecoveryCodes.Add(new TotpRecoveryCodeEntityModel
                {
                    Id = Guid.NewGuid(),
                    CreatedBy = userId,
                    CodeHash = HashRecoveryCode(formatted),
                    IsUsed = false,
                    CreatedAtUtc = now,
                    UsedAtUtc = null
                });
            }

            return plaintextCodes;
        }

        private async Task<string> GenerateRecoveryCode()
        {
            var chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            var length = await _systemSettings.GetIntAsync(SystemSettingKeyEnum.Policies_Totp_RecoveryCodeFormatting_NumberOfCharactersInAGroup) * await _systemSettings.GetIntAsync(SystemSettingKeyEnum.Policies_Totp_RecoveryCodeFormatting_NumberOfGroupsInARecoveryCode);

            var bytes = new byte[length];
            RandomNumberGenerator.Fill(bytes);

            var result = new char[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = chars[bytes[i] % chars.Length];
            }

            return new string(result);
        }

        private async Task<string> FormatRecoveryCode(string raw)
        {
            var groupSize = await _systemSettings.GetIntAsync(SystemSettingKeyEnum.Policies_Totp_RecoveryCodeFormatting_NumberOfCharactersInAGroup);
            var sb = new StringBuilder();

            for (int i = 0; i < raw.Length; i++)
            {
                if (i > 0 && i % groupSize == 0)
                    sb.Append('-');
                sb.Append(raw[i]);
            }

            return sb.ToString();
        }

        private static string HashRecoveryCode(string code)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(code));
            return Convert.ToHexString(bytes).ToLowerInvariant();
        }
        #endregion
        #region TOTP
        private static bool VerifyCode(string base32Secret, string code)
        {
            if (string.IsNullOrWhiteSpace(code) || code.Length != CodeDigits)
                return false;

            if (!int.TryParse(code, out _))
                return false;

            var secretBytes = Base32Encoding.ToBytes(base32Secret);
            var totp = new Totp(secretBytes, step: TimeStepSeconds, totpSize: CodeDigits);

            return totp.VerifyTotp(code, out _, new VerificationWindow(
                previous: ToleranceSteps,
                future: ToleranceSteps
            ));
        }
        private async Task<string> GetProvisioningUri(string base32Secret, string userEmail)
        {
            var encodedIssuer = Uri.EscapeDataString(await _systemSettings.GetStringAsync(SystemSettingKeyEnum.Policies_Totp_TotpIssuer));
            var encodedEmail = Uri.EscapeDataString(userEmail);

            return $"otpauth://totp/{encodedIssuer}:{encodedEmail}" +
                   $"?secret={base32Secret}" +
                   $"&issuer={encodedIssuer}" +
                   $"&algorithm=SHA1" +
                   $"&digits={CodeDigits}" +
                   $"&period={TimeStepSeconds}";
        }
        #endregion
    }
}
