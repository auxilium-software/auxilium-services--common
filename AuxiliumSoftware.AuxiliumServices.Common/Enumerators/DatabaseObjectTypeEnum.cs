using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.Common.Enumerators
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum DatabaseObjectTypeEnum
    {
        // ####################################################################################################
        // Global -> Tenant
        // ####################################################################################################
        [JsonPropertyName("Global.Tenant.Tenant")]
        Global_Tenant_Tenant,



        // ####################################################################################################
        // Within Tenancy -> Calendar
        // ####################################################################################################
        [JsonPropertyName("WithinTenancy.Calendar.Event")]
        WithinTenancy_Calendar_Event,
        [JsonPropertyName("WithinTenancy.Calendar.EventInvite")]
        WithinTenancy_Calendar_EventInvite,



        // ####################################################################################################
        // Within Tenancy -> Case
        // ####################################################################################################
        [JsonPropertyName("WithinTenancy.Case.Case")]
        WithinTenancy_Case,
        [JsonPropertyName("WithinTenancy.Case.AdditionalProperty")]
        WithinTenancy_Case_AdditionalProperty,
        [JsonPropertyName("WithinTenancy.Case.Client")]
        WithinTenancy_Case_Client,
        [JsonPropertyName("WithinTenancy.Case.File")]
        WithinTenancy_Case_File,
        [JsonPropertyName("WithinTenancy.Case.Message")]
        WithinTenancy_Case_Message,
        [JsonPropertyName("WithinTenancy.Case.TimelineEntry")]
        WithinTenancy_Case_TimelineEntry,
        [JsonPropertyName("WithinTenancy.Case.Todo")]
        WithinTenancy_Case_Todo,
        [JsonPropertyName("WithinTenancy.Case.Worker")]
        WithinTenancy_Case_Worker,



        // ####################################################################################################
        // Within Tenancy -> Enumerator
        // ####################################################################################################
        [JsonPropertyName("WithinTenancy.Enumerator.Enumerator")]
        WithinTenancy_Enumerator,
        [JsonPropertyName("WithinTenancy.Enumerator.Translation")]
        WithinTenancy_Enumerator_Translation,
        [JsonPropertyName("WithinTenancy.Enumerator.Value")]
        WithinTenancy_Enumerator_Value,
        [JsonPropertyName("WithinTenancy.Enumerator.ValueTranslation")]
        WithinTenancy_Enumerator_ValueTranslation,



        // ####################################################################################################
        // Within Tenancy -> Log
        // ####################################################################################################
        [JsonPropertyName("WithinTenancy.Log.CaseMessageReadBy.EventEntry")]
        WithinTenancy_Log_CaseMessageReadBy_EventEntry,
        [JsonPropertyName("WithinTenancy.Log.CaseModification.EventEntry")]
        WithinTenancy_Log_CaseModification_EventEntry,

        [JsonPropertyName("WithinTenancy.Log.CaseMerge.RowChange.EventEntry")]
        WithinTenancy_Log_CaseMerge_RowChange_EventEntry,
        [JsonPropertyName("WithinTenancy.Log.CaseMerge.EventEntry")]
        WithinTenancy_Log_CaseMerge_EventEntry,

        [JsonPropertyName("WithinTenancy.Log.LoginAttempt.EventEntry")]
        WithinTenancy_Log_LoginAttempt_EventEntry,

        [JsonPropertyName("WithinTenancy.Log.SystemMessageQueue.EmailSent.EventEntry")]
        WithinTenancy_Log_SystemMessageQueue_EmailSent_EventEntry,
        [JsonPropertyName("WithinTenancy.Log.SystemMessageQueue.FailedAction.EventEntry")]
        WithinTenancy_Log_SystemMessageQueue_FailedAction_EventEntry,

        [JsonPropertyName("WithinTenancy.Log.SystemBulletin.EntryDismissal.EventEntry")]
        WithinTenancy_Log_SystemBulletin_EntryDismissal_EventEntry,
        [JsonPropertyName("WithinTenancy.Log.SystemBulletin.EntryView.EventEntry")]
        WithinTenancy_Log_SystemBulletin_EntryView_EventEntry,

        [JsonPropertyName("WithinTenancy.Log.UserModification.EventEntry")]
        WithinTenancy_Log_UserModification_EventEntry,



        // ####################################################################################################
        // Within Tenancy -> System
        // ####################################################################################################
        [JsonPropertyName("WithinTenancy.System.BulletinEntry")]
        WithinTenancy_System_BulletinEntry,
        [JsonPropertyName("WithinTenancy.System.MetricEntry")]
        WithinTenancy_System_MetricEntry,
        [JsonPropertyName("WithinTenancy.System.SettingEntry")]
        WithinTenancy_System_SettingEntry,
        [JsonPropertyName("WithinTenancy.System.Waf.IpBlacklistEntry")]
        WithinTenancy_System_Waf_IpBlacklistEntry,
        [JsonPropertyName("WithinTenancy.System.Waf.IpWhitelistEntry")]
        WithinTenancy_System_Waf_IpWhitelistEntry,
        [JsonPropertyName("WithinTenancy.System.Waf.UserBlacklistEntry")]
        WithinTenancy_System_Waf_UserBlacklistEntry,
        [JsonPropertyName("WithinTenancy.System.Waf.UserWhitelistEntry")]
        WithinTenancy_System_Waf_UserWhitelistEntry,



        // ####################################################################################################
        // Within Tenancy -> User
        // ####################################################################################################
        [JsonPropertyName("WithinTenancy.User.User")]
        WithinTenancy_User,
        [JsonPropertyName("WithinTenancy.User.AdditionalProperty")]
        WithinTenancy_User_AdditionalProperty,
        [JsonPropertyName("WithinTenancy.User.File")]
        WithinTenancy_User_File,
        [JsonPropertyName("WithinTenancy.User.PasswordSetToken")]
        WithinTenancy_User_PasswordSetToken,
        [JsonPropertyName("WithinTenancy.User.RefreshToken")]
        WithinTenancy_User_RefreshToken,
        [JsonPropertyName("WithinTenancy.User.TotpRecoveryCode")]
        WithinTenancy_User_TotpRecoveryCode,
        [JsonPropertyName("WithinTenancy.User.WemwbsAssessment")]
        WithinTenancy_User_WemwbsAssessment,
    }
}
