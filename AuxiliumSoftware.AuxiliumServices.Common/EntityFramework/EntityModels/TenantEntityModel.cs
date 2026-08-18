using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels
{
    public class TenantEntityModel
    {
        [Key]
        public required Guid Id { get; set; }

        public required string Domain { get; set; }

        public required TenantLifecycleStateEnum LifecycleStatus { get; set; }





        public ICollection<CalendarEventEntityModel>? CalendarEvents { get; set; }
        public ICollection<CalendarEventInviteEntityModel>? CalendarEventInvites { get; set; }
        public ICollection<CaseEntityModel>? Cases { get; set; }
        public ICollection<CaseAdditionalPropertyEntityModel>? CaseAdditionalProperties { get; set; }
        public ICollection<CaseClientEntityModel>? CaseClients { get; set; }
        public ICollection<CaseFileEntityModel>? CaseFiles { get; set; }
        public ICollection<CaseMessageEntityModel>? CaseMessages { get; set; }
        public ICollection<CaseTimelineEntryEntityModel>? CaseTimelineEntries { get; set; }
        public ICollection<CaseTodoEntityModel>? CaseTodos { get; set; }
        public ICollection<CaseWorkerEntityModel>? CaseWorkers { get; set; }
        public ICollection<DataEnumeratorEntityModel>? DataEnumerator_Enumerators { get; set; }
        public ICollection<DataEnumeratorTranslationEntityModel>? DataEnumerator_EnumeratorTranslations { get; set; }
        public ICollection<DataEnumeratorValueEntityModel>? DataEnumerator_EnumeratorValues { get; set; }
        public ICollection<DataEnumeratorValueTranslationEntityModel>? DataEnumerator_EnumeratorValueTranslations { get; set; }
        public ICollection<LogCaseMessageReadByEventEntityModel>? Log_CaseMessageReadBys { get; set; }
        public ICollection<LogCaseModificationEventEntityModel>? Log_CaseModificationEvents { get; set; }
        public ICollection<LogLoginAttemptEventEntityModel>? Log_LoginAttempts { get; set; }
        public ICollection<LogSystemBulletinEntryDismissalEventEntityModel>? Log_SystemBulletinEntryDismissals { get; set; }
        public ICollection<LogSystemBulletinEntryViewEventEntityModel>? Log_SystemBulletinEntryViews { get; set; }
        public ICollection<LogSystemMessageQueueSentEmailEntityModel>? Log_SystemMessageQueueSentEmail { get; set; }
        public ICollection<LogSystemMessageQueueFailedActionEntityModel>? Log_SystemMessageQueueFailedActions { get; set; }
        public ICollection<LogUserModificationEventEntityModel>? Log_UserModificationEvents { get; set; }
        public ICollection<SystemBulletinEntryEntityModel>? System_Bulletins { get; set; }
        public ICollection<SystemSettingEntityModel>? System_Settings { get; set; }
        public ICollection<SystemWafIpBlacklistEntryEntityModel>? System_Waf_IpBlacklist { get; set; }
        public ICollection<SystemWafIpWhitelistEntryEntityModel>? System_Waf_IpWhitelist { get; set; }
        public ICollection<SystemWafUserBlacklistEntryEntityModel>? System_Waf_UserBlacklist { get; set; }
        public ICollection<SystemWafUserWhitelistEntryEntityModel>? System_Waf_UserWhitelist { get; set; }
        // public ICollection<TenantEntityModel>? Tenants { get; set; }
        public ICollection<UserEntityModel>? Users { get; set; }
        public ICollection<UserAdditionalPropertyEntityModel>? UserAdditionalProperties { get; set; }
        public ICollection<UserFileEntityModel>? UserFiles { get; set; }
        public ICollection<PasswordSetTokenEntityModel>? UserPasswordSetTokens { get; set; }
        public ICollection<RefreshTokenEntityModel>? UserRefreshTokens { get; set; }
        public ICollection<TotpRecoveryCodeEntityModel>? UserTotpRecoveryCodes { get; set; }
        public ICollection<WemwbsAssessmentEntityModel>? UserWemwbsAssessments { get; set; }
    }
}
