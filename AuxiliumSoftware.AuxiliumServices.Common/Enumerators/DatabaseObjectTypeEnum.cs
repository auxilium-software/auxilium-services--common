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
        [JsonPropertyName("Case.TimelineEntry")]
        Case_TimelineEntry,
        [JsonPropertyName("Case.Todo")]
        Case_Todo,
        [JsonPropertyName("Case.Worker")]
        Case_Worker,





        [JsonPropertyName("Log.CaseMessageReadBy.EventEntry")]
        Log_CaseMessageReadBy_EventEntry,
        [JsonPropertyName("Log.CaseModification.EventEntry")]
        Log_CaseModification_EventEntry,
        [JsonPropertyName("Log.LoginAttempt.EventEntry")]
        Log_LoginAttempt_EventEntry,
        [JsonPropertyName("Log.SystemMessageQueue.EmailSent.EventEntry")]
        Log_SystemMessageQueue_EmailSent_EventEntry,
        [JsonPropertyName("Log.SystemMessageQueue.FailedAction.EventEntry")]
        Log_SystemMessageQueue_FailedAction_EventEntry,
        [JsonPropertyName("Log.SystemBulletin.EntryDismissal.EventEntry")]
        Log_SystemBulletin_EntryDismissal_EventEntry,
        [JsonPropertyName("Log.SystemBulletin.EntryView.EventEntry")]
        Log_SystemBulletin_EntryView_EventEntry,
        [JsonPropertyName("Log.UserModification.EventEntry")]
        Log_UserModification_EventEntry,





        [JsonPropertyName("System.BulletinEntry")]
        System_BulletinEntry,
        [JsonPropertyName("System.MetricEntry")]
        System_MetricEntry,
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
