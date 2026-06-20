using AuxiliumSoftware.AuxiliumServices.Common.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SystemSettingKeyEnum
    {
        // ####################################################################################################
        // Policies -> Web Application Firewall
        // ####################################################################################################
        // Web Application Firewall
        [JsonPropertyName("policies.webApplicationFirewall.enabled")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Bool)]
        [SystemSettingDefaultValueAttribute(true)]
        [SystemSettingDescriptionAttribute("Acts as a master switch for the Web Application Firewall. When enabled, the system monitors login attempts, detects brute-force attacks, and automatically blacklists suspicious IP addresses.")]
        [SystemSettingRecommendationAttribute("Should always be set to true in production environments.")]
        Policies_WebApplicationFirewall_Enabled,

        // Web Application Firewall - User
        [JsonPropertyName("policies.webApplicationFirewall.user.maximumFailedLoginsPerUser")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Int)]
        [SystemSettingDefaultValueAttribute(5)]
        [SystemSettingDescriptionAttribute("The number of failed login attempts allowed for a single user account before the account is temporarily locked. This protects against targeted password guessing attacks on specific accounts.")]
        [SystemSettingRecommendationAttribute("5-10 attempts is typical. Lower values increase security but may frustrate legitimate users who mistype passwords.")]
        Policies_WebApplicationFirewall_User_MaximumFailedLoginsPerUser,

        [JsonPropertyName("policies.webApplicationFirewall.user.userLockoutWindowInMinutes")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Int)]
        [SystemSettingDefaultValueAttribute(15)]
        [SystemSettingDescriptionAttribute("The time window in which failed login attempts are counted. If the maximum failed attempts occur within this window, the account is locked. After this window passes without reaching the threshold, the counter resets.")]
        [SystemSettingRecommendationAttribute("10-15 minutes balances security with usability. Shorter windows are more forgiving to users but less secure.")]
        Policies_WebApplicationFirewall_User_UserLockoutWindowInMinutes,

        [JsonPropertyName("policies.webApplicationFirewall.user.userLockoutDurationInMinutes")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Int)]
        [SystemSettingDefaultValueAttribute(30)]
        [SystemSettingDescriptionAttribute("How long a user account remains locked after exceeding the maximum failed login attempts. The user cannot log in during this period, even with the correct password.")]
        [SystemSettingRecommendationAttribute("15-30 minutes balances security with user convenience. Longer durations deter attackers but may require admin intervention for legitimate users.")]
        Policies_WebApplicationFirewall_User_UserLockoutDurationInMinutes,

        // Web Application Firewall - IP
        [JsonPropertyName("policies.webApplicationFirewall.ip.maximumFailedLoginsPerIp")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Int)]
        [SystemSettingDefaultValueAttribute(10)]
        [SystemSettingDescriptionAttribute("The number of failed login attempts allowed from a single IP address before that IP is temporarily blacklisted. This protects against attackers trying multiple usernames from the same location.")]
        [SystemSettingRecommendationAttribute("10-20 attempts. Set higher than per-user limit to avoid blacklisting shared networks such as offices, universities, or NAT gateways.")]
        Policies_WebApplicationFirewall_Ip_MaximumFailedLoginsPerIp,

        [JsonPropertyName("policies.webApplicationFirewall.ip.ipBlacklistWindowInMinutes")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Int)]
        [SystemSettingDefaultValueAttribute(10)]
        [SystemSettingDescriptionAttribute("The time window in which failed login attempts from an IP address are counted. If the maximum failed attempts from one IP occur within this window, the IP is blacklisted.")]
        [SystemSettingRecommendationAttribute("10-15 minutes is typical. Shorter windows may allow slow-and-steady attacks to evade detection.")]
        Policies_WebApplicationFirewall_Ip_IpBlacklistWindowInMinutes,

        [JsonPropertyName("policies.webApplicationFirewall.ip.temporaryBlacklistsBeforePermanentBlacklist")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Int)]
        [SystemSettingDefaultValueAttribute(3)]
        [SystemSettingDescriptionAttribute("The number of times blacklists IP can be temporarily blacklisted before it receives a permanent ban. This escalates punishment for persistent attackers who return after temporary blacklists expire.")]
        [SystemSettingRecommendationAttribute("3-5 temporary blacklists before permanent ban. Lower values may catch legitimate users on shared IPs; higher values give attackers more attempts.")]
        Policies_WebApplicationFirewall_Ip_TemporaryBlacklistsBeforePermanentBlacklist,

        [JsonPropertyName("policies.webApplicationFirewall.ip.permanentBanWindowHours")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Int)]
        [SystemSettingDefaultValueAttribute(24)]
        [SystemSettingDescriptionAttribute("The time window in which repeated temporary blacklists are counted toward a permanent ban. If an IP receives the threshold number of temporary blacklists within this window, it is permanently banned.")]
        [SystemSettingRecommendationAttribute("24 hours is typical. Shorter windows may allow attackers to evade permanent bans by waiting; longer windows may catch users who have legitimate issues over time.")]
        Policies_WebApplicationFirewall_Ip_PermanentBanWindowHours,

        // Web Application Firewall - Listing
        [JsonPropertyName("policies.webApplicationFirewall.listing.temporaryIpWhitelistDurationInMinutes.default")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Int)]
        [SystemSettingDefaultValueAttribute(1440)] // 24 hours
        [SystemSettingDescriptionAttribute("The default duration for temporary IP whitelist entries. Used when an administrator adds an IP to the whitelist without specifying a custom duration.")]
        [SystemSettingRecommendationAttribute("1440 minutes (24 hours) to 10080 minutes (7 days). Temporary whitelists are useful for contractors, testing, or temporary office locations.")]
        Policies_WebApplicationFirewall_Listing_TemporaryIpWhitelistDurationInMinutes_Default,

        [JsonPropertyName("policies.webApplicationFirewall.listing.temporaryUserWhitelistDurationInMinutes.default")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Int)]
        [SystemSettingDefaultValueAttribute(1440)] // 24 hours
        [SystemSettingDescriptionAttribute("The default duration for temporary user whitelist entries. Whitelisted users bypass WAF lockout checks during this period.")]
        [SystemSettingRecommendationAttribute("1440 minutes (24 hours) to 10080 minutes (7 days). Useful for users experiencing repeated lockouts due to legitimate issues.")]
        Policies_WebApplicationFirewall_Listing_TemporaryUserWhitelistDurationInMinutes_Default,

        [JsonPropertyName("policies.webApplicationFirewall.listing.temporaryIpBlacklistDurationInMinutes.default")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Int)]
        [SystemSettingDefaultValueAttribute(60)] // 1 hour
        [SystemSettingDescriptionAttribute("The default duration for temporary IP blacklist entries created manually by administrators. Distinct from automatic temporary blacklists triggered by failed login thresholds.")]
        [SystemSettingRecommendationAttribute("60-1440 minutes. Manual blacklists are typically for known bad actors, so longer durations than automatic blacklists are appropriate.")]
        Policies_WebApplicationFirewall_Listing_TemporaryIpBlacklistDurationInMinutes_Default,

        [JsonPropertyName("policies.webApplicationFirewall.listing.temporaryUserBlacklistDurationInMinutes.default")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Int)]
        [SystemSettingDefaultValueAttribute(60)] // 1 hour
        [SystemSettingDescriptionAttribute("The default duration for temporary user blacklist entries created manually by administrators. The user cannot log in during this period.")]
        [SystemSettingRecommendationAttribute("60-1440 minutes. Use for suspected compromised accounts pending investigation, or as a disciplinary measure.")]
        Policies_WebApplicationFirewall_Listing_TemporaryUserBlacklistDurationInMinutes_Default,

        [JsonPropertyName("policies.webApplicationFirewall.listing.temporaryIpBlacklistDurationInMinutes.failedLoginAttempts")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Int)]
        [SystemSettingDefaultValueAttribute(15)]
        [SystemSettingDescriptionAttribute("How long an IP address remains blacklisted after exceeding the failed login threshold. All login attempts from this IP will be rejected during the blacklist period.")]
        [SystemSettingRecommendationAttribute("15-60 minutes for temporary blacklists. Shorter durations are more forgiving for shared IPs; longer durations deter persistent attackers.")]
        Policies_WebApplicationFirewall_Listing_TemporaryIpBlacklistDurationInMinutes_FailedLoginAttempts,

        // Web Application Firewall - Rate Limiting
        [JsonPropertyName("policies.webApplicationFirewall.rateLimiting.enabled")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Bool)]
        [SystemSettingDefaultValueAttribute(true)]
        [SystemSettingDescriptionAttribute("Controls whether login requests are rate-limited. When enabled, the system enforces delays and limits on how frequently login attempts can be made, slowing down automated attacks.")]
        [SystemSettingRecommendationAttribute("Should be enabled to prevent rapid-fire automated attacks and credential stuffing.")]
        Policies_WebApplicationFirewall_RateLimiting_Enabled,

        [JsonPropertyName("policies.webApplicationFirewall.rateLimiting.enableRateLimitingBasedUponIpAddress")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Bool)]
        [SystemSettingDefaultValueAttribute(true)]
        [SystemSettingDescriptionAttribute("When enabled, rate limits are applied per IP address. This limits how quickly any single IP can attempt logins, regardless of which accounts they target.")]
        [SystemSettingRecommendationAttribute("Should generally be enabled. This is the primary defence against distributed password spraying attacks.")]
        Policies_WebApplicationFirewall_RateLimiting_RateLimitBasedUponIpAddress,

        [JsonPropertyName("policies.webApplicationFirewall.rateLimiting.enableRateLimitingBasedUponUserAccount")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Bool)]
        [SystemSettingDefaultValueAttribute(false)]
        [SystemSettingDescriptionAttribute("When enabled, rate limits are also applied per user account. This limits how quickly login attempts can be made against a specific account, even from multiple IPs.")]
        [SystemSettingRecommendationAttribute("Enable if you experience distributed attacks targeting specific accounts. May cause issues if legitimate users have network problems causing rapid retries.")]
        Policies_WebApplicationFirewall_RateLimiting_RateLimitBasedUponUserAccount,

        [JsonPropertyName("policies.webApplicationFirewall.rateLimiting.maximumLoginRequestsPerMinute")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Int)]
        [SystemSettingDefaultValueAttribute(10)]
        [SystemSettingDescriptionAttribute("The maximum number of login attempts allowed per minute from a single source. Requests exceeding this limit are rejected or delayed.")]
        [SystemSettingRecommendationAttribute("10-20 per minute is reasonable for most applications. Lower values provide more protection but may affect users with connectivity issues.")]
        Policies_WebApplicationFirewall_RateLimiting_MaximumLoginRequestsPerMinute,

        [JsonPropertyName("policies.webApplicationFirewall.rateLimiting.failureDelayBaseMilliseconds")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Int)]
        [SystemSettingDefaultValueAttribute(500)]
        [SystemSettingDescriptionAttribute("The base delay in milliseconds added after a failed login attempt. This slows down automated attacks by making each attempt take longer. The delay may increase with consecutive failures.")]
        [SystemSettingRecommendationAttribute("500-1000ms is typical. Higher values slow attacks more but may feel sluggish to legitimate users who mistype passwords.")]
        Policies_WebApplicationFirewall_RateLimiting_FailureDelayBaseMilliseconds,

        [JsonPropertyName("policies.webApplicationFirewall.rateLimiting.failureDelayMaximumMilliseconds")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Int)]
        [SystemSettingDefaultValueAttribute(5000)]
        [SystemSettingDescriptionAttribute("The maximum delay in milliseconds that can be applied after failed login attempts. This caps the exponential backoff to prevent excessively long waits.")]
        [SystemSettingRecommendationAttribute("5000-10000ms (5-10 seconds) is typical. This significantly slows automated attacks while remaining tolerable for humans.")]
        Policies_WebApplicationFirewall_RateLimiting_FailureDelayMaximumMilliseconds,

        // Web Application Firewall - Distributed Attack Detection
        [JsonPropertyName("policies.webApplicationFirewall.distributedAttackDetection.enabled")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Bool)]
        [SystemSettingDefaultValueAttribute(true)]
        [SystemSettingDescriptionAttribute("Detects credential stuffing attacks where an attacker targets a single account from many different IP addresses to evade per-IP limits. When triggered, can lock the targeted account.")]
        [SystemSettingRecommendationAttribute("Enable if you handle sensitive data or have experienced credential stuffing attacks. Essential for high-value targets.")]
        Policies_WebApplicationFirewall_DistributedAttackDetection_Enabled,

        [JsonPropertyName("policies.webApplicationFirewall.distributedAttackDetection.failedLoginsFromDistinctIpsThreshold")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Int)]
        [SystemSettingDefaultValueAttribute(5)]
        [SystemSettingDescriptionAttribute("If a single user account receives failed login attempts from this many different IP addresses within the detection window, it triggers distributed attack protection and the account may be locked.")]
        [SystemSettingRecommendationAttribute("5-10 distinct IPs. Legitimate users rarely log in from many different IPs simultaneously. Lower values catch attacks sooner but may have false positives for travelling users.")]
        Policies_WebApplicationFirewall_DistributedAttackDetection_FailedLoginsFromDistinctIpsThreshold,

        [JsonPropertyName("policies.webApplicationFirewall.distributedAttackDetection.windowInMinutes")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Int)]
        [SystemSettingDefaultValueAttribute(10)]
        [SystemSettingDescriptionAttribute("The time window in which failed logins from distinct IPs are counted. If the threshold is reached within this window, distributed attack protection is triggered.")]
        [SystemSettingRecommendationAttribute("10-30 minutes. Shorter windows may miss slow distributed attacks; longer windows may catch legitimate users who travel or use VPNs.")]
        Policies_WebApplicationFirewall_DistributedAttackDetection_WindowInMinutes,

        // ####################################################################################################
        // Policies -> Password
        // ####################################################################################################
        [JsonPropertyName("policies.password.minimumLength")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Int)]
        [SystemSettingDefaultValueAttribute(8)]
        [SystemSettingDescriptionAttribute("The minimum number of characters required for user passwords. Each additional character exponentially increases the time required for brute-force attacks.")]
        [SystemSettingRecommendationAttribute("12-16 characters recommended. NIST guidelines suggest 8 as an absolute minimum, but longer passwords provide significantly better security.")]
        Policies_Password_MinimumLength,

        [JsonPropertyName("policies.password.requireUppercase")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Bool)]
        [SystemSettingDefaultValueAttribute(true)]
        [SystemSettingDescriptionAttribute("When enabled, passwords must contain at least one uppercase letter (A-Z). This increases the character set attackers must consider when guessing passwords.")]
        [SystemSettingRecommendationAttribute("Enable for defence in depth. However, password length matters more than complexity requirements.")]
        Policies_Password_RequireUppercase,

        [JsonPropertyName("policies.password.requireLowercase")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Bool)]
        [SystemSettingDefaultValueAttribute(true)]
        [SystemSettingDescriptionAttribute("When enabled, passwords must contain at least one lowercase letter (a-z). Most passwords naturally contain lowercase letters.")]
        [SystemSettingRecommendationAttribute("Generally enable. This requirement is rarely problematic as most users naturally use lowercase letters.")]
        Policies_Password_RequireLowercase,

        [JsonPropertyName("policies.password.requireNumeric")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Bool)]
        [SystemSettingDefaultValueAttribute(true)]
        [SystemSettingDescriptionAttribute("When enabled, passwords must contain at least one numeric digit (0-9). This expands the character set beyond just letters.")]
        [SystemSettingRecommendationAttribute("Enable, but be aware this may encourage predictable patterns like appending '1' to passwords.")]
        Policies_Password_RequireNumeric,

        [JsonPropertyName("policies.password.requireSpecialCharacter")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Bool)]
        [SystemSettingDefaultValueAttribute(true)]
        [SystemSettingDescriptionAttribute("When enabled, passwords must contain at least one special character such as !@#$%^&*. This significantly expands the character set.")]
        [SystemSettingRecommendationAttribute("Consider carefully. Can cause usability issues without major security benefit if length requirements are strong. NIST no longer recommends mandatory special characters.")]
        Policies_Password_RequireSpecialCharacter,

        [JsonPropertyName("policies.password.maximumAgeInDays")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Int)]
        [SystemSettingDefaultValueAttribute(365)]
        [SystemSettingDescriptionAttribute("The number of days before a password expires and the user must create a new one. Set to 0 to disable password expiration entirely.")]
        [SystemSettingRecommendationAttribute("NIST now recommends against forced rotation (set to 0) unless a breach is suspected. Frequent changes lead to weaker passwords and predictable patterns.")]
        Policies_Password_MaximumAgeInDays,

        [JsonPropertyName("policies.password.allowCommonPasswords")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Bool)]
        [SystemSettingDefaultValueAttribute(false)]
        [SystemSettingDescriptionAttribute("When disabled, the system checks new passwords against a list of commonly-used passwords (such as 'password123', 'qwerty', 'letmein') and rejects matches.")]
        [SystemSettingRecommendationAttribute("Keep disabled (false) to prevent easily-guessable passwords. The common password list should be updated periodically.")]
        Policies_Password_AllowCommonPasswords,

        [JsonPropertyName("policies.password.preventPasswordReuse")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Bool)]
        [SystemSettingDefaultValueAttribute(true)]
        [SystemSettingDescriptionAttribute("When enabled, users cannot reuse their previous passwords. The system maintains a history of password hashes to enforce this policy.")]
        [SystemSettingRecommendationAttribute("Enable to prevent users cycling back to old, potentially compromised passwords. Typically combined with a password history count.")]
        Policies_Password_PreventPasswordReuse,

        [JsonPropertyName("policies.password.resetTokenLength")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Int)]
        [SystemSettingDefaultValueAttribute(32)]
        [SystemSettingDescriptionAttribute("The length of the password reset token in bytes.")]
        [SystemSettingRecommendationAttribute("Set to a minimum of 32 bytes for security.")]
        Policies_Password_PasswordResetTokenLength,

        // ####################################################################################################
        // Policies -> TOTP
        // ####################################################################################################
        [JsonPropertyName("policies.totp.requireForAdministrators")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Bool)]
        [SystemSettingDefaultValueAttribute(true)]
        [SystemSettingDescriptionAttribute("When enabled, administrator accounts must configure and use TOTP two-factor authentication. Administrators cannot access admin functions without completing MFA.")]
        [SystemSettingRecommendationAttribute("Strongly recommended. Administrator accounts are high-value targets and should always require MFA.")]
        Policies_Totp_RequireForAdministrators,

        [JsonPropertyName("policies.totp.requireForAllNonAdministrators")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Bool)]
        [SystemSettingDefaultValueAttribute(false)]
        [SystemSettingDescriptionAttribute("When enabled, all non-administrator users must also configure TOTP two-factor authentication. This provides organisation-wide MFA coverage.")]
        [SystemSettingRecommendationAttribute("Enable if handling sensitive data or required by compliance frameworks such as ISO 27001, GDPR, or sector-specific regulations.")]
        Policies_Totp_RequireForAllNonAdministrators,

        [JsonPropertyName("policies.totp.totpIssuer")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.String)]
        [SystemSettingDefaultValueAttribute("Auxilium")]
        [SystemSettingDescriptionAttribute("The name displayed in authenticator apps to identify this application. Helps users distinguish between multiple TOTP entries.")]
        [SystemSettingRecommendationAttribute("Use your organisation or application name. Keep it short and recognisable.")]
        Policies_Totp_TotpIssuer,

        [JsonPropertyName("policies.totp.numberOfRecoveryCodesToGenerate")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Int)]
        [SystemSettingDefaultValueAttribute(8)]
        [SystemSettingDescriptionAttribute("The number of one-time recovery codes generated when a user sets up TOTP. These codes allow account recovery if the user loses access to their authenticator device.")]
        [SystemSettingRecommendationAttribute("8-10 codes provides a good balance between recovery options and security. Users should store these securely offline.")]
        Policies_Totp_NumberOfRecoveryCodesToGenerate,

        [JsonPropertyName("policies.totp.recoveryCodeFormatting.numberOfCharactersInAGroup")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Int)]
        [SystemSettingDefaultValueAttribute(5)]
        [SystemSettingDescriptionAttribute("The number of characters in each group of the recovery codes.")]
        [SystemSettingRecommendationAttribute("5 characters per group is recommended for readability.")]
        Policies_Totp_RecoveryCodeFormatting_NumberOfCharactersInAGroup,

        [JsonPropertyName("policies.totp.recoveryCodeFormatting.numberOfGroupsInARecoveryCode")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Int)]
        [SystemSettingDefaultValueAttribute(5)]
        [SystemSettingDescriptionAttribute("The number of groups in the recovery codes.")]
        [SystemSettingRecommendationAttribute("5 groups is recommended for readability.")]
        Policies_Totp_RecoveryCodeFormatting_NumberOfGroupsInARecoveryCode,

        // ####################################################################################################
        // Policies -> Account Lockout
        // ####################################################################################################
        [JsonPropertyName("policies.accountLockout.enabled")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Bool)]
        [SystemSettingDefaultValueAttribute(true)]
        [SystemSettingDescriptionAttribute("Master switch for the account lockout system. When enabled, accounts are automatically locked after too many failed login attempts, independent of WAF IP-based blacklisting.")]
        [SystemSettingRecommendationAttribute("Should be enabled to protect against brute-force attacks. Works in conjunction with WAF for layered defence.")]
        Policies_AccountLockout_Enabled,

        [JsonPropertyName("policies.accountLockout.maximumFailedLoginAttempts")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Int)]
        [SystemSettingDefaultValueAttribute(5)]
        [SystemSettingDescriptionAttribute("The number of consecutive failed login attempts before an account is locked. This counter is per-account regardless of which IP the attempts come from.")]
        [SystemSettingRecommendationAttribute("5-10 attempts balances security with usability. Consider your user base - external-facing systems may need stricter limits.")]
        Policies_AccountLockout_MaximumFailedLoginAttempts,

        [JsonPropertyName("policies.accountLockout.lockoutDurationInMinutes")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Int)]
        [SystemSettingDefaultValueAttribute(30)]
        [SystemSettingDescriptionAttribute("How long an account remains locked after exceeding the failed attempt threshold. Set to 0 for permanent lockout requiring administrator intervention to unlock.")]
        [SystemSettingRecommendationAttribute("15-30 minutes for automatic unlock. Use 0 (permanent) if manual review of lockouts is required for compliance or security policy.")]
        Policies_AccountLockout_LockoutDurationInMinutes,

        [JsonPropertyName("policies.accountLockout.resetFailedAttemptsAfterInMinutes")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Int)]
        [SystemSettingDefaultValueAttribute(15)]
        [SystemSettingDescriptionAttribute("The time period after which the failed login counter resets to zero if no further failed attempts occur. This prevents accumulation of occasional typos over time.")]
        [SystemSettingRecommendationAttribute("15-30 minutes. Should be shorter than or equal to the lockout window to provide meaningful protection.")]
        Policies_AccountLockout_ResetFailedAttemptsAfterInMinutes,

        [JsonPropertyName("policies.accountLockout.notifyUserUponLockout")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Bool)]
        [SystemSettingDefaultValueAttribute(true)]
        [SystemSettingDescriptionAttribute("When enabled, sends an email to the user when their account is locked due to failed login attempts. This alerts users to potential compromise attempts against their account.")]
        [SystemSettingRecommendationAttribute("Enable to inform users of suspicious activity. The email should include guidance on what to do if they did not cause the lockout.")]
        Policies_AccountLockout_NotifyUserUponLockout,

        [JsonPropertyName("policies.accountLockout.notifyAdministratorsUponLockout")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Bool)]
        [SystemSettingDefaultValueAttribute(false)]
        [SystemSettingDescriptionAttribute("When enabled, administrators receive email notifications when any account is locked. Useful for monitoring potential attacks across the system.")]
        [SystemSettingRecommendationAttribute("Enable for high-security environments. May be noisy in large organisations - consider filtering to only notify for admin account lockouts.")]
        Policies_AccountLockout_NotifyAdministratorsUponLockout,

        // ####################################################################################################
        // Policies -> Logging
        // ####################################################################################################
        // Logging - Security
        [JsonPropertyName("policies.logging.security.logSuccessfulLogins")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Bool)]
        [SystemSettingDefaultValueAttribute(true)]
        [SystemSettingDescriptionAttribute("Records an audit log entry every time a user successfully logs in.")]
        [SystemSettingRecommendationAttribute("Enable for security auditing and compliance requirements. Essential for investigating account compromises and unusual access patterns.")]
        Policies_Logging_Security_LogSuccessfulLogins,

        [JsonPropertyName("policies.logging.security.logIncorrectPasswordLoginAttempts")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Bool)]
        [SystemSettingDefaultValueAttribute(true)]
        [SystemSettingDescriptionAttribute("Records when a login fails due to an incorrect password for an existing account. Logs the username and IP but never the attempted password.")]
        [SystemSettingRecommendationAttribute("Enable to detect brute-force attempts and potential credential compromise. Critical for security monitoring.")]
        Policies_Logging_Security_LogIncorrectPasswordLoginAttempts,

        [JsonPropertyName("policies.logging.security.logIncorrectEmailAddressLoginAttempts")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Bool)]
        [SystemSettingDefaultValueAttribute(false)]
        [SystemSettingDescriptionAttribute("Records when a login fails because the email address does not exist in the system. Useful for detecting enumeration attacks attempting to discover valid accounts.")]
        [SystemSettingRecommendationAttribute("Enable to detect account enumeration attacks. Be aware this may generate significant log volume if attackers probe many addresses.")]
        Policies_Logging_Security_LogIncorrectEmailAddressLoginAttempts,

        [JsonPropertyName("policies.logging.security.logPasswordChanges")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Bool)]
        [SystemSettingDefaultValueAttribute(true)]
        [SystemSettingDescriptionAttribute("Records an audit entry when any user changes their password. Does not log the actual passwords (old or new), only the event metadata.")]
        [SystemSettingRecommendationAttribute("Enable for security auditing. Helps detect account takeovers where attackers change passwords after gaining access.")]
        Policies_Logging_Security_LogPasswordChanges,

        // Logging - Entity Actions - Cases
        [JsonPropertyName("policies.logging.entityActions.cases.logCreations")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Bool)]
        [SystemSettingDefaultValueAttribute(true)]
        [SystemSettingDescriptionAttribute("Records an audit entry when a new case is created. Includes who created the case, when, and basic case metadata.")]
        [SystemSettingRecommendationAttribute("Enable for audit trail compliance. Required for most regulatory frameworks involving case management.")]
        Policies_Logging_EntityActions_Cases_LogCreations,

        [JsonPropertyName("policies.logging.entityActions.cases.logModifications")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Bool)]
        [SystemSettingDefaultValueAttribute(true)]
        [SystemSettingDescriptionAttribute("Records an audit entry when case details are modified. Can include field-level change tracking depending on configuration.")]
        [SystemSettingRecommendationAttribute("Enable to maintain a complete audit trail of case changes. Essential for compliance and dispute resolution.")]
        Policies_Logging_EntityActions_Cases_LogModifications,

        [JsonPropertyName("policies.logging.entityActions.cases.logDeletions")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Bool)]
        [SystemSettingDefaultValueAttribute(true)]
        [SystemSettingDescriptionAttribute("Records an audit entry when a case is deleted or archived. This log persists even after the case data is removed.")]
        [SystemSettingRecommendationAttribute("Always enable. Deletion logging is critical for compliance and detecting unauthorised data removal.")]
        Policies_Logging_EntityActions_Cases_LogDeletions,

        // Logging - Entity Actions - Case Additional Properties
        [JsonPropertyName("policies.logging.entityActions.caseAdditionalProperties.logCreations")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Bool)]
        [SystemSettingDefaultValueAttribute(true)]
        [SystemSettingDescriptionAttribute("Records when custom properties or metadata are added to a case. Useful for tracking additional data fields beyond core case information.")]
        [SystemSettingRecommendationAttribute("Enable if custom properties contain sensitive or regulated information requiring audit trails.")]
        Policies_Logging_EntityActions_CaseAdditionalProperties_LogCreations,

        [JsonPropertyName("policies.logging.entityActions.caseAdditionalProperties.logModifications")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Bool)]
        [SystemSettingDefaultValueAttribute(true)]
        [SystemSettingDescriptionAttribute("Records when custom properties on a case are modified. Captures the change event and who made it.")]
        [SystemSettingRecommendationAttribute("Enable for complete case audit trails, especially for properties affecting case status or outcomes.")]
        Policies_Logging_EntityActions_CaseAdditionalProperties_LogModifications,

        [JsonPropertyName("policies.logging.entityActions.caseAdditionalProperties.logDeletions")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Bool)]
        [SystemSettingDefaultValueAttribute(true)]
        [SystemSettingDescriptionAttribute("Records when custom properties are removed from a case. Important for tracking data removal.")]
        [SystemSettingRecommendationAttribute("Enable to ensure no data can be silently removed from cases without an audit record.")]
        Policies_Logging_EntityActions_CaseAdditionalProperties_LogDeletions,

        // Logging - Entity Actions - Case Workers
        [JsonPropertyName("policies.logging.entityActions.caseWorkers.logAssignments")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Bool)]
        [SystemSettingDefaultValueAttribute(true)]
        [SystemSettingDescriptionAttribute("Records when a caseworker is assigned to a case. Tracks who made the assignment and when.")]
        [SystemSettingRecommendationAttribute("Enable for workload tracking, accountability, and compliance requirements around case handling.")]
        Policies_Logging_EntityActions_CaseWorkers_LogAssignments,

        [JsonPropertyName("policies.logging.entityActions.caseWorkers.logUnAssignments")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Bool)]
        [SystemSettingDefaultValueAttribute(true)]
        [SystemSettingDescriptionAttribute("Records when a caseworker is removed from a case. Maintains history of all worker involvement.")]
        [SystemSettingRecommendationAttribute("Enable to maintain complete case assignment history for accountability and handover tracking.")]
        Policies_Logging_EntityActions_CaseWorkers_LogUnAssignments,

        // Logging - Entity Actions - Case Clients
        [JsonPropertyName("policies.logging.entityActions.caseClients.logAssignments")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Bool)]
        [SystemSettingDefaultValueAttribute(true)]
        [SystemSettingDescriptionAttribute("Records when a client is associated with a case. Important for tracking client-case relationships.")]
        [SystemSettingRecommendationAttribute("Enable for data protection compliance and maintaining accurate records of client involvement.")]
        Policies_Logging_EntityActions_CaseClients_LogAssignments,

        [JsonPropertyName("policies.logging.entityActions.caseClients.logUnAssignments")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Bool)]
        [SystemSettingDefaultValueAttribute(true)]
        [SystemSettingDescriptionAttribute("Records when a client is disassociated from a case. Maintains complete history of client relationships.")]
        [SystemSettingRecommendationAttribute("Enable to track all changes to client-case associations for compliance and audit purposes.")]
        Policies_Logging_EntityActions_CaseClients_LogUnAssignments,

        // Logging - Entity Actions - Case Messages
        [JsonPropertyName("policies.logging.entityActions.caseMessages.logSends")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Bool)]
        [SystemSettingDefaultValueAttribute(true)]
        [SystemSettingDescriptionAttribute("Records when messages are sent within a case. Tracks sender, recipient, and timestamp without necessarily storing message content in the log.")]
        [SystemSettingRecommendationAttribute("Enable for communication audit trails. May be required for regulatory compliance in case management.")]
        Policies_Logging_EntityActions_CaseMessages_LogSends,

        [JsonPropertyName("policies.logging.entityActions.caseMessages.logViews")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Bool)]
        [SystemSettingDefaultValueAttribute(true)]
        [SystemSettingDescriptionAttribute("Records when case messages are viewed. Useful for confirming message delivery and read receipts.")]
        [SystemSettingRecommendationAttribute("Enable if message acknowledgment is important for your workflow or compliance requirements.")]
        Policies_Logging_EntityActions_CaseMessages_LogViews,

        // Logging - Entity Actions - Case Files
        [JsonPropertyName("policies.logging.entityActions.caseFiles.logUploads")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Bool)]
        [SystemSettingDefaultValueAttribute(true)]
        [SystemSettingDescriptionAttribute("Records when files are uploaded to a case. Tracks uploader, filename, size, and timestamp.")]
        [SystemSettingRecommendationAttribute("Enable for document management compliance and tracking the origin of case evidence or documentation.")]
        Policies_Logging_EntityActions_CaseFiles_LogUploads,

        [JsonPropertyName("policies.logging.entityActions.caseFiles.logViews")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Bool)]
        [SystemSettingDefaultValueAttribute(true)]
        [SystemSettingDescriptionAttribute("Records when case files are viewed or downloaded. Important for tracking access to sensitive documents.")]
        [SystemSettingRecommendationAttribute("Enable for data protection compliance. Essential when case files contain personal or sensitive information.")]
        Policies_Logging_EntityActions_CaseFiles_LogViews,

        [JsonPropertyName("policies.logging.entityActions.caseFiles.logDeletions")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Bool)]
        [SystemSettingDefaultValueAttribute(true)]
        [SystemSettingDescriptionAttribute("Records when files are deleted from a case. This audit record persists after the file itself is removed.")]
        [SystemSettingRecommendationAttribute("Always enable. Critical for detecting unauthorised document removal and maintaining evidence integrity.")]
        Policies_Logging_EntityActions_CaseFiles_LogDeletions,

        // Logging - Entity Actions - Case Timeline Entries
        [JsonPropertyName("policies.logging.entityActions.caseTimelineEntries.logCreations")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Bool)]
        [SystemSettingDefaultValueAttribute(true)]
        [SystemSettingDescriptionAttribute("Records when timeline entries are created for a case. Tracks entry details and timestamps.")]
        [SystemSettingRecommendationAttribute("Enable for workflow tracking and accountability in case management processes.")]
        Policies_Logging_EntityActions_CaseTimelineEntries_LogCreations,

        [JsonPropertyName("policies.logging.entityActions.caseTimelineEntries.logModifications")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Bool)]
        [SystemSettingDefaultValueAttribute(true)]
        [SystemSettingDescriptionAttribute("Records when case timeline entries are modified, including status changes, reassignments, or deadline updates.")]
        [SystemSettingRecommendationAttribute("Enable to track timeline entry progress and changes for performance monitoring and compliance.")]
        Policies_Logging_EntityActions_CaseTimelineEntries_LogModifications,

        [JsonPropertyName("policies.logging.entityActions.caseTimelineEntries.logDeletions")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Bool)]
        [SystemSettingDefaultValueAttribute(true)]
        [SystemSettingDescriptionAttribute("Records when case timeline entries are deleted. Maintains audit trail even after entry removal.")]
        [SystemSettingRecommendationAttribute("Enable to prevent silent removal of assigned work and maintain complete workflow history.")]
        Policies_Logging_EntityActions_CaseTimelineEntries_LogDeletions,

        // Logging - Entity Actions - Case Todos
        [JsonPropertyName("policies.logging.entityActions.caseTodos.logCreations")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Bool)]
        [SystemSettingDefaultValueAttribute(true)]
        [SystemSettingDescriptionAttribute("Records when tasks or to-do items are created for a case. Tracks task assignment and deadlines.")]
        [SystemSettingRecommendationAttribute("Enable for workflow tracking and accountability in case management processes.")]
        Policies_Logging_EntityActions_CaseTodos_LogCreations,

        [JsonPropertyName("policies.logging.entityActions.caseTodos.logModifications")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Bool)]
        [SystemSettingDefaultValueAttribute(true)]
        [SystemSettingDescriptionAttribute("Records when case tasks are modified, including status changes, reassignments, or deadline updates.")]
        [SystemSettingRecommendationAttribute("Enable to track task progress and changes for performance monitoring and compliance.")]
        Policies_Logging_EntityActions_CaseTodos_LogModifications,

        [JsonPropertyName("policies.logging.entityActions.caseTodos.logDeletions")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Bool)]
        [SystemSettingDefaultValueAttribute(true)]
        [SystemSettingDescriptionAttribute("Records when case tasks are deleted. Maintains audit trail even after task removal.")]
        [SystemSettingRecommendationAttribute("Enable to prevent silent removal of assigned work and maintain complete workflow history.")]
        Policies_Logging_EntityActions_CaseTodos_LogDeletions,

        // Logging - Entity Actions - Users
        [JsonPropertyName("policies.logging.entityActions.users.logCreations")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Bool)]
        [SystemSettingDefaultValueAttribute(true)]
        [SystemSettingDescriptionAttribute("Records when new user accounts are created. Includes who created the account and initial role assignments.")]
        [SystemSettingRecommendationAttribute("Always enable. Essential for access control auditing and detecting unauthorised account creation.")]
        Policies_Logging_EntityActions_Users_LogCreations,

        [JsonPropertyName("policies.logging.entityActions.users.logModifications")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Bool)]
        [SystemSettingDefaultValueAttribute(true)]
        [SystemSettingDescriptionAttribute("Records when user account details are modified, including role changes, profile updates, and permission changes.")]
        [SystemSettingRecommendationAttribute("Always enable. Critical for tracking privilege escalation and account changes.")]
        Policies_Logging_EntityActions_Users_LogModifications,

        [JsonPropertyName("policies.logging.entityActions.users.logDeletions")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Bool)]
        [SystemSettingDefaultValueAttribute(true)]
        [SystemSettingDescriptionAttribute("Records when user accounts are deleted or deactivated. This audit record persists for compliance purposes.")]
        [SystemSettingRecommendationAttribute("Always enable. Required for access control compliance and investigating former user activities.")]
        Policies_Logging_EntityActions_Users_LogDeletions,

        // Logging - Entity Actions - User Additional Properties
        [JsonPropertyName("policies.logging.entityActions.userAdditionalProperties.logCreations")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Bool)]
        [SystemSettingDefaultValueAttribute(true)]
        [SystemSettingDescriptionAttribute("Records when custom properties are added to user profiles. Useful for tracking additional user metadata.")]
        [SystemSettingRecommendationAttribute("Enable if custom user properties contain sensitive information or affect system access.")]
        Policies_Logging_EntityActions_UserAdditionalProperties_LogCreations,

        [JsonPropertyName("policies.logging.entityActions.userAdditionalProperties.logModifications")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Bool)]
        [SystemSettingDefaultValueAttribute(true)]
        [SystemSettingDescriptionAttribute("Records when custom user properties are modified. Tracks changes to extended user profile information.")]
        [SystemSettingRecommendationAttribute("Enable for complete user profile audit trails, especially for properties affecting access or roles.")]
        Policies_Logging_EntityActions_UserAdditionalProperties_LogModifications,

        [JsonPropertyName("policies.logging.entityActions.userAdditionalProperties.logDeletions")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Bool)]
        [SystemSettingDefaultValueAttribute(true)]
        [SystemSettingDescriptionAttribute("Records when custom properties are removed from user profiles. Maintains audit trail of all profile changes.")]
        [SystemSettingRecommendationAttribute("Enable to ensure user profile changes are fully auditable.")]
        Policies_Logging_EntityActions_UserAdditionalProperties_LogDeletions,

        // Logging - Entity Actions - User Files
        [JsonPropertyName("policies.logging.entityActions.userFiles.logUploads")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Bool)]
        [SystemSettingDefaultValueAttribute(true)]
        [SystemSettingDescriptionAttribute("Records when users upload files to their own profiles or storage areas. Tracks uploader, filename, and metadata.")]
        [SystemSettingRecommendationAttribute("Enable for storage management and tracking the origin of user-uploaded content.")]
        Policies_Logging_EntityActions_UserFiles_LogUploads,

        [JsonPropertyName("policies.logging.entityActions.userFiles.logViews")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Bool)]
        [SystemSettingDefaultValueAttribute(true)]
        [SystemSettingDescriptionAttribute("Records when user files are accessed or downloaded. Important for tracking access to personal documents.")]
        [SystemSettingRecommendationAttribute("Enable for data protection compliance, especially when user files may contain personal information.")]
        Policies_Logging_EntityActions_UserFiles_LogViews,

        [JsonPropertyName("policies.logging.entityActions.userFiles.logDeletions")]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.Bool)]
        [SystemSettingDefaultValueAttribute(true)]
        [SystemSettingDescriptionAttribute("Records when user files are deleted. The audit record persists after the file is removed.")]
        [SystemSettingRecommendationAttribute("Enable to maintain complete file lifecycle audit trails and detect unauthorised deletions.")]
        Policies_Logging_EntityActions_UserFiles_LogDeletions,










        // ####################################################################################################
        // Instance
        // ####################################################################################################
        [JsonPropertyName("instance.fqdn")]
        [SystemSettingVisibilityAttribute(SystemSettingVisibilityEnum.Public)]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.String)]
        [SystemSettingDescriptionAttribute("The fully qualified domain name for this instance (for example, 'app.example.com'). Used for generating URLs in emails, API responses, and other external communications.")]
        [SystemSettingRecommendationAttribute("Set to your production domain. Ensure this matches your SSL certificate and DNS configuration.")]
        Instance_Fqdn,

        [JsonPropertyName("instance.branding.logoRelativePath")]
        [SystemSettingVisibilityAttribute(SystemSettingVisibilityEnum.Public)]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.String)]
        [SystemSettingDefaultValueAttribute("AuxiliumLogo.png")]
        [SystemSettingDescriptionAttribute("The relative path to the organisation's logo image, used in the header and emails. Should be an image optimised for web display on light backgrounds.")]
        [SystemSettingRecommendationAttribute("Use a PNG or SVG format for best quality. Recommended dimensions are approximately 200x50 pixels.")]
        Instance_Branding_LogoRelativePath,

        [JsonPropertyName("instance.branding.logoContrastRelativePath")]
        [SystemSettingVisibilityAttribute(SystemSettingVisibilityEnum.Public)]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.String)]
        [SystemSettingDefaultValueAttribute("AuxiliumLogo-White.png")]
        [SystemSettingDescriptionAttribute("The relative path to a contrast version of the logo, used on dark backgrounds or in contexts where the primary logo would not be visible.")]
        [SystemSettingRecommendationAttribute("Provide a white or light-coloured version of your logo for use on dark navigation bars or footers.")]
        Instance_Branding_LogoContrastRelativePath,

        [JsonPropertyName("instance.branding.name")]
        [SystemSettingVisibilityAttribute(SystemSettingVisibilityEnum.Public)]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.String)]
        [SystemSettingDescriptionAttribute("The display name for this instance, shown in the header, browser title, emails, and other user-facing locations. This is your organisation or application name.")]
        [SystemSettingRecommendationAttribute("Use your organisation name or a clear application identifier. Keep it concise for display in constrained spaces.")]
        Instance_Branding_Name,

        // contacts
        [JsonPropertyName("instance.contacts.firstPointOfContact.emailAddress")]
        [SystemSettingVisibilityAttribute(SystemSettingVisibilityEnum.Public)]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.String)]
        [SystemSettingDescriptionAttribute("The primary support email address displayed to users for general enquiries and assistance. This is the first contact point users see when they need help.")]
        [SystemSettingRecommendationAttribute("Use a monitored support mailbox rather than an individual's email address for continuity and coverage.")]
        Instance_Contacts_FirstPointOfContact_EmailAddress,

        [JsonPropertyName("instance.contacts.firstPointOfContact.phone.number")]
        [SystemSettingVisibilityAttribute(SystemSettingVisibilityEnum.Public)]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.String)]
        [SystemSettingDescriptionAttribute("The primary support phone number displayed to users. Include the international dialling code for clarity.")]
        [SystemSettingRecommendationAttribute("Format consistently with country code (for example, +44 1234567890). Consider using a dedicated support line.")]
        Instance_Contacts_FirstPointOfContact_Phone_Number,

        [JsonPropertyName("instance.contacts.firstPointOfContact.phone.openingHours")]
        [SystemSettingVisibilityAttribute(SystemSettingVisibilityEnum.Public)]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.String)]
        [SystemSettingDescriptionAttribute("The hours during which the phone support line is staffed. Displayed alongside the phone number to set user expectations.")]
        [SystemSettingRecommendationAttribute("Be specific about time zones (for example, 'Monday-Friday, 09:00-17:00 GMT'). Update for bank holidays as needed.")]
        Instance_Contacts_FirstPointOfContact_Phone_OpeningHours,

        [JsonPropertyName("instance.contacts.firstPointOfContact.text.number")]
        [SystemSettingVisibilityAttribute(SystemSettingVisibilityEnum.Public)]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.String)]
        [SystemSettingDescriptionAttribute("A phone number that accepts SMS text messages for support. Useful for users who prefer text-based communication.")]
        [SystemSettingRecommendationAttribute("Only configure if you have the capability to monitor and respond to SMS messages. Leave blank if not supported.")]
        Instance_Contacts_FirstPointOfContact_Text_Number,

        [JsonPropertyName("instance.contacts.firstPointOfContact.text.openingHours")]
        [SystemSettingVisibilityAttribute(SystemSettingVisibilityEnum.Public)]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.String)]
        [SystemSettingDescriptionAttribute("The hours during which SMS support is available. Displayed alongside the text number.")]
        [SystemSettingRecommendationAttribute("Be clear about response time expectations for text-based support.")]
        Instance_Contacts_FirstPointOfContact_Text_OpeningHours,

        [JsonPropertyName("instance.contacts.maintainer.name")]
        [SystemSettingVisibilityAttribute(SystemSettingVisibilityEnum.Public)]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.String)]
        [SystemSettingDescriptionAttribute("The name of the organisation or team responsible for technical maintenance of this system. Displayed in technical notices and system information pages.")]
        [SystemSettingRecommendationAttribute("Use the IT team or department name, or the name of your managed service provider if applicable.")]
        Instance_Contacts_Maintainer_Name,

        [JsonPropertyName("instance.contacts.maintainer.emailAddress")]
        [SystemSettingVisibilityAttribute(SystemSettingVisibilityEnum.Public)]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.String)]
        [SystemSettingDescriptionAttribute("Email address for technical and maintenance enquiries. Used for system-related issues rather than general support.")]
        [SystemSettingRecommendationAttribute("Use a technical team mailbox. This may be different from general support to ensure proper routing of technical issues.")]
        Instance_Contacts_Maintainer_EmailAddress,

        [JsonPropertyName("instance.contacts.operator.name")]
        [SystemSettingVisibilityAttribute(SystemSettingVisibilityEnum.Public)]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.String)]
        [SystemSettingDescriptionAttribute("The name of the organisation operating this system. This may be different from the maintainer if the system is operated on behalf of another organisation.")]
        [SystemSettingRecommendationAttribute("Use the legal name of the organisation responsible for data processing under GDPR or similar regulations.")]
        Instance_Contacts_Operator_Name,

        [JsonPropertyName("instance.contacts.operator.emailAddress")]
        [SystemSettingVisibilityAttribute(SystemSettingVisibilityEnum.Public)]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.String)]
        [SystemSettingDescriptionAttribute("Official contact email for the operating organisation. May be used for formal enquiries, data protection requests, and official correspondence.")]
        [SystemSettingRecommendationAttribute("Use an official organisational email address that is monitored for compliance and legal correspondence.")]
        Instance_Contacts_Operator_EmailAddress,

        [JsonPropertyName("instance.contacts.generalEnquiries.name")]
        [SystemSettingVisibilityAttribute(SystemSettingVisibilityEnum.Public)]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.String)]
        [SystemSettingDescriptionAttribute("Display name for general enquiries contact information. May be shown as an alternative to support for non-technical questions.")]
        [SystemSettingRecommendationAttribute("Use a department or team name appropriate for general public or client enquiries.")]
        Instance_Contacts_GeneralEnquiries_Name,

        [JsonPropertyName("instance.contacts.generalEnquiries.emailAddress")]
        [SystemSettingVisibilityAttribute(SystemSettingVisibilityEnum.Public)]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.String)]
        [SystemSettingDescriptionAttribute("Email address for general enquiries not related to technical support. Appropriate for business, partnership, or general information requests.")]
        [SystemSettingRecommendationAttribute("Use a monitored general enquiries mailbox that routes to appropriate teams.")]
        Instance_Contacts_GeneralEnquiries_EmailAddress,

        // defaults
        [JsonPropertyName("instance.defaults.timeZone")]
        [SystemSettingVisibilityAttribute(SystemSettingVisibilityEnum.Public)]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.String)]
        [SystemSettingDescriptionAttribute("The default time zone for displaying dates and times when a user has not set their personal preference. Uses IANA time zone database identifiers.")]
        [SystemSettingRecommendationAttribute("Set to your primary user base's time zone (for example, 'Europe/London'). Users can override in their preferences.")]
        Instance_Defaults_TimeZone,

        [JsonPropertyName("instance.defaults.language")]
        [SystemSettingVisibilityAttribute(SystemSettingVisibilityEnum.Public)]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.String)]
        [SystemSettingDescriptionAttribute("The default interface language for new users and unauthenticated pages. Use locale (for example, 'en-GB', 'cy-GB').")]
        [SystemSettingRecommendationAttribute("Set to your primary user base's language. Users can override in their personal preferences if multilingual support is enabled.")]
        Instance_Defaults_Language,

        // navigation
        [JsonPropertyName("instance.navigation.portalBaseUrl")]
        [SystemSettingVisibilityAttribute(SystemSettingVisibilityEnum.Public)]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.String)]
        [SystemSettingDescriptionAttribute("The base URL for the portal.")]
        [SystemSettingRecommendationAttribute("https://portal.veteranslegallink.org")]
        Instance_Navigation_PortalBaseUrl,

        [JsonPropertyName("instance.navigation.aboutPageRelativePath")]
        [SystemSettingVisibilityAttribute(SystemSettingVisibilityEnum.Public)]
        [SystemSettingExpectedValueTypeAttribute(SystemSettingValueTypeEnum.String)]
        [SystemSettingDescriptionAttribute("The relative path to the 'About' or information page for this instance.")]
        [SystemSettingRecommendationAttribute("Create a page explaining your organisation and the purpose of this system. Important for transparency and trust.")]
        Instance_Navigation_AboutPageRelativePath,
    }
}
