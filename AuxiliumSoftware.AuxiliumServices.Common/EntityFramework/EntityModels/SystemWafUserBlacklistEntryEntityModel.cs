using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels
{
    public class SystemWafUserBlacklistEntryEntityModel
    {
        /// <summary>
        /// The unique identifier for the User Block.
        /// </summary>
        [Key]
        public required Guid Id { get; set; }

        /// <summary>
        /// The timestamp of when the User Block was created.
        /// </summary>
        public required DateTime CreatedAt { get; set; }

        /// <summary>
        /// The unique identifier of the User who created the User Block.
        /// </summary>
        public Guid? CreatedBy { get; set; }





        /// <summary>
        /// The unique identifier for the User that is blocked.
        /// </summary>
        public required Guid UserId { get; set; }

        /// <summary>
        /// A reason for why the User was blocked.
        /// </summary>
        public required string JustificationForBlacklist { get; set; }

        /// <summary>
        /// Whether the User Block is permanent (i.e. does not expire and has to be manually removed) or temporary (i.e. expires at a certain date and time).
        /// </summary>
        public required bool IsPermanent { get; set; }





        /// <summary>
        /// When the User Block expires, if it is not permanent.
        /// This MUST be null if the User Block is permanent.
        /// </summary>
        public DateTime? ExpiresAt { get; set; }

        /// <summary>
        /// For a temporary block, this denotes when the block was lifted (i.e. when the User Block became inactive).
        /// This MUST be null if the User Block is permanent.
        /// This MUST be null if the User Block is temporary and has not yet been lifted.
        /// </summary>
        public DateTime? UnblacklistedAt { get; set; }

        /// <summary>
        /// The unique identifier of the User who lifted the block, if applicable.
        /// This MUST be null if the User Block is permanent.
        /// This MUST be null if the User Block is temporary and has not yet been lifted.
        /// </summary>
        public Guid? UnblacklistedBy { get; set; }

        /// <summary>
        /// A reason for why the User was unblocked.
        /// </summary>
        public string? JustificationForUnblacklist { get; set; }





        public UserEntityModel? CreatedByUser { get; set; }
        public UserEntityModel? BlockedUser { get; set; }
        public UserEntityModel? UnblockedByUser { get; set; }
    }
}
