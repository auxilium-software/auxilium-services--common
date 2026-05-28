using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.Common.Enumerators
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum DatabaseObjectTypeEnum
    {

        [JsonPropertyName("Case.Cases")]
        Case,
        [JsonPropertyName("Case.AdditionalProperty")]
        Case_AdditionalProperty,
        [JsonPropertyName("Case.Client")]
        Case_Client,
        [JsonPropertyName("Case.File")]
        Case_File,
        [JsonPropertyName("Case.Message")]
        Case_Message,
        [JsonPropertyName("Case.Todo")]
        Case_Todo,
        [JsonPropertyName("Case.Worker")]
        Case_Worker,





        [JsonPropertyName("Log.CaseMessageReadByEventEntry")]
        Log_CaseMessageReadByEventEntry,
        [JsonPropertyName("Log.CaseModificationEventEntry")]
        Log_CaseModificationEventEntry,
        [JsonPropertyName("Log.LoginAttemptEventEntry")]
        Log_LoginAttemptEventEntry,
        [JsonPropertyName("Log.SystemBulletinEntryDismissalEventEntry")]
        Log_SystemBulletinEntryDismissalEventEntry,
        [JsonPropertyName("Log.SystemBulletinEntryViewEventEntry")]
        Log_SystemBulletinEntryViewEventEntry,
        [JsonPropertyName("Log.UserModificationEventEntry")]
        Log_UserModificationEventEntry,





        [JsonPropertyName("System.BulletinEntry")]
        System_BulletinEntry,
        [JsonPropertyName("System.SettingEntry")]
        System_SettingEntry,
        [JsonPropertyName("System.Waf.IpBlacklistEntry")]
        System_Waf_IpBlacklistEntry,
        [JsonPropertyName("System.Waf.IpWhitelistEntry")]
        System_Waf_IpWhitelistEntry,
        [JsonPropertyName("System.Waf.UserBlacklistEntry")]
        System_Waf_UserBlacklistEntry,
        [JsonPropertyName("System.Waf.UserWhitelistEntry")]
        System_Waf_UserWhitelistEntry,




        [JsonPropertyName("User.Users")]
        User,
        [JsonPropertyName("User.AdditionalProperty")]
        User_AdditionalProperty,
        [JsonPropertyName("User.File")]
        User_File,
        [JsonPropertyName("User.PasswordSetToken")]
        User_PasswordSetToken,
        [JsonPropertyName("User.RefreshToken")]
        User_RefreshToken,
        [JsonPropertyName("User.TotpRecoveryCode")]
        User_TotpRecoveryCode,
        [JsonPropertyName("User.WemwbsAssessment")]
        User_WemwbsAssessment,
    }
}
