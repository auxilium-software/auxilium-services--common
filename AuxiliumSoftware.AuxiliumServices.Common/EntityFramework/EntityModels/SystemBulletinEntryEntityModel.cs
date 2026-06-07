using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels
{
    public class SystemBulletinEntryEntityModel
    {
        /// <summary>
        /// The unique identifier for the System Bulletin Entry.
        /// </summary>
        [Key]
        public required Guid Id { get; set; }
        /// <summary>
        /// The timestamp of when the System Bulletin Entry was created.
        /// </summary>
        public required DateTime CreatedAtUtc { get; set; }
        /// <summary>
        /// The unique identifier for the User who created the System Bulletin Entry.
        /// </summary>
        public Guid? CreatedBy { get; set; }



        /// <summary>
        /// An enumerator representing the Severity of the System Bulletin.
        /// </summary>
        /// <example>
        /// So for example, system maintenance might have a Severity of "Warning".
        /// </example>
        public required SystemBulletinMessageSeverityEnum Severity { get; set; }
        /// <summary>
        /// The title for the System Bulletin Entry.
        /// </summary>
        public required string Title { get; set; }
        /// <summary>
        /// The actual content of the System Bulletin Entry.
        /// </summary>
        public required string Content { get; set; }
        /// <summary>
        /// Whether the System Bulletin Entry should be served.
        /// </summary>
        public required bool IsActive { get; set; }
        /// <summary>
        /// Whether Users are able to confirm that they've read the System Bulletin Entry and don't want to see it again.
        /// </summary>
        public required bool IsDismissible { get; set; }
        /// <summary>
        /// The DateTime representing when the System Bulletin Entry is due to go live, the default value in database is the current DateTime.
        /// This is useful for scheduling System Bulletin Entries.
        /// </summary>
        public required DateTime StartsAtUtc { get; set; }
        /// <summary>
        /// An optional DateTime representing when the System Bulletin Entry is due to no longer be live.
        /// This is useful for scheduling System Bulletin Entries.
        /// </summary>
        public DateTime? EndsAtUtc { get; set; }
        /// <summary>
        /// An enumerator representing who the Target Audience is.
        /// </summary>
        /// <example>
        /// If you only wanted logged in users to see this System Bulletin Entry, then you could set this to "LoggedInUsersOnly".
        /// </example>
        public SystemBulletinMessageTargetAudienceEnum TargetAudience { get; set; }
        /// <summary>
        /// If the TargetAudience enumerator has been set to "SingleUserOnly", then this is where you'd specify the unique identifier for that User.
        /// </summary>
        public Guid? SpecificUserId { get; set; }





        /// <summary>
        /// The User that created the System Bulletin Entry.
        /// </summary>
        public UserEntityModel? CreatedByUser { get; set; }
        /// <summary>
        /// The specific user the bulletin is targeted at (if TargetAudience has been set to "LoggedInUsersOnly").
        /// </summary>
        public UserEntityModel? SpecificUser { get; set; }





        /// <summary>
        /// A Log of Users who have dismissed the System Bulletin.
        /// </summary>
        public ICollection<LogSystemBulletinEntryDismissalEventEntityModel>? Dismissals { get; set; }
        /// <summary>
        /// A Log of Users who have viewed the System Bulletin.
        /// </summary>
        public ICollection<LogSystemBulletinEntryViewEventEntityModel>? Views { get; set; }
    }
}
