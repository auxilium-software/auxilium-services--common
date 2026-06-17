using System.ComponentModel.DataAnnotations;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels
{
    public class UserEntityModel
    {
        /// <summary>
        /// The unique identifier for the User.
        /// </summary>
        [Key]
        public required Guid Id { get; set; }
        /// <summary>
        /// The timestamp of when the User was created.
        /// </summary>
        public required DateTime CreatedAtUtc { get; set; }
        /// <summary>
        /// The unique identifier of the User who created the User.
        /// </summary>
        public Guid? CreatedBy { get; set; }
        /// <summary>
        /// The timestamp of when the User was last updated.
        /// </summary>
        public DateTime? LastUpdatedAtUtc { get; set; }
        /// <summary>
        /// The unique identifier of the User who last updated the User.
        /// </summary>
        public Guid? LastUpdatedBy { get; set; }





        /// <summary>
        /// The email address of the User.
        /// </summary>
        public required string? EmailAddress { get; set; }
        /// <summary>
        /// The hashed password of the User.
        /// </summary>
        public required string? PasswordHash { get; set; }
        /// <summary>
        /// The full name of the User.
        /// </summary>
        public required string FullName { get; set; }
        /// <summary>
        /// The full address of the User.
        /// </summary>
        public required string? FullAddress { get; set; }
        /// <summary>
        /// The telephone number of the User.
        /// </summary>
        public required string? TelephoneNumber { get; set; }
        /// <summary>
        /// The gender of the User.
        /// </summary>
        public required string? Gender { get; set; }
        /// <summary>
        /// The date of birth of the User.
        /// </summary>
        public required DateOnly? DateOfBirth { get; set; }
        /// <summary>
        /// How the User found out about our service.
        /// </summary>
        public required string? HowDidYouFindOutAboutOurService { get; set; }
        /// <summary>
        /// The preferred language for the User.
        /// </summary>
        public required string LanguagePreference { get; set; }





        /// <summary>
        /// Whether the User has confirmed their Email Address.
        /// </summary>
        public required bool HasEmailAddressBeenVerified { get; set; } = false;
        /// <summary>
        /// Whether the User is allowed to log in.
        /// </summary>
        public required bool AllowLogin { get; set; } = false;
        /// <summary>
        /// Whether the User has to change their password.
        /// </summary>
        public required bool MustChangePassword { get; set; } = false;





        /// <summary>
        /// Whether the User has the "Case Worker" Role.
        /// </summary>
        public required bool IsCaseWorker { get; set; } = false;
        /// <summary>
        /// Whether the User has the "Case Worker Manager" Role.
        /// </summary>
        public required bool IsCaseWorkerManager { get; set; } = false;
        /// <summary>
        /// Whether the User has the "Administrator" Role.
        /// </summary>
        public required bool IsAdministrator { get; set; } = false;





        /// <summary>
        /// Base32-encoded TOTP shared secret.
        /// MUST be Null if the user has never started TOTP setup.
        /// There MUST be a value but with TotpEnabled=false during pending setup.
        /// </summary>
        public string? TotpSecret { get; set; }

        /// <summary>
        /// Whether TOTP is active and enforced for this user.
        /// </summary>
        public bool TotpEnabled { get; set; }

        /// <summary>
        /// A timestamp of when the TOTP was activated (after verifying the first code).
        /// MUST be null if not enabled.
        /// </summary>
        public DateTime? TotpEnabledAtUtc { get; set; }





        /// <summary>
        /// Whether the User has requested to delete their account.
        /// </summary>
        public required bool DeletionRequested { get; set; } = false;

        /// <summary>
        /// An optional reason provided by the User for requesting account deletion.
        /// </summary>
        public string? DeletionRequestReason { get; set; }





        /// <summary>
        /// The User that created the User.
        /// </summary>
        public UserEntityModel? CreatedByUser { get; set; }
        /// <summary>
        /// The User that last updated the User.
        /// </summary>
        public UserEntityModel? LastUpdatedByUser { get; set; }





        /// <summary>
        /// Cases where the User is assigned as a worker.
        /// </summary>
        public ICollection<CaseWorkerEntityModel>? WorkerOnCases { get; set; }
        /// <summary>
        /// Cases where the User is a client.
        /// </summary>
        public ICollection<CaseClientEntityModel>? ClientOnCases { get; set; }





        /// <summary>
        /// Files belonging to the User.
        /// </summary>
        public ICollection<UserFileEntityModel>? Files { get; set; }
        /// <summary>
        /// Additional Properties for the User.
        /// </summary>
        public ICollection<UserAdditionalPropertyEntityModel>? AdditionalProperties { get; set; }
        /// <summary>
        /// Refresh Tokens for the User.
        /// </summary>
        public ICollection<RefreshTokenEntityModel>? RefreshTokens { get; set; }





        /// <summary>
        /// Messages sent by the User.
        /// </summary>
        public ICollection<CaseMessageEntityModel>? SentMessages { get; set; }
        /// <summary>
        /// Message Read Receipts for the User.
        /// </summary>
        public ICollection<LogCaseMessageReadByEventEntityModel>? MessageReadReceipts { get; set; }





        /// <summary>
        /// Todos assigned to the User.
        /// </summary>
        public ICollection<CaseTodoEntityModel>? AssignedTodos { get; set; }
        /// <summary>
        /// Todos completed by the User.
        /// </summary>
        public ICollection<CaseTodoEntityModel>? CompletedTodos { get; set; }





        /// <summary>
        /// </summary>
        public ICollection<WemwbsAssessmentEntityModel>? CreatedWEMWBSAssessments { get; set; }
        /// <summary>
        /// WEMWBS Assessments for the User.
        /// </summary>
        public ICollection<WemwbsAssessmentEntityModel>? WEMWBSAssessments { get; set; }





        /// <summary>
        /// System Bulletins targeted specifically for the User.
        /// </summary>
        public ICollection<SystemBulletinEntryEntityModel>? TargetedBulletins { get; set; }
        /// <summary>
        /// System Bulletin dismissals by the User.
        /// </summary>
        public ICollection<LogSystemBulletinEntryDismissalEventEntityModel>? BulletinDismissals { get; set; }
        /// <summary>
        /// System Bulletin views by the User.
        /// </summary>
        public ICollection<LogSystemBulletinEntryViewEventEntityModel>? BulletinViews { get; set; }
        /// <summary>
        /// A Log of Modification Events related to the User.
        /// </summary>
        public ICollection<LogUserModificationEventEntityModel>? EventLog { get; set; }
        /// <summary>
        /// A list of TOTP Recovery Codes belonging to the User.
        /// </summary>
        public ICollection<TotpRecoveryCodeEntityModel>? TotpRecoveryCodes { get; set; }
        /// <summary>
        /// Password Reset Tokens the User has created.
        /// </summary>
        public ICollection<PasswordSetTokenEntityModel>? CreatedPasswordSetTokens { get; set; }
        /// <summary>
        /// Password Reset Tokens issued to the User.
        /// </summary>
        public ICollection<PasswordSetTokenEntityModel>? PasswordSetTokens { get; set; }
    }
}
