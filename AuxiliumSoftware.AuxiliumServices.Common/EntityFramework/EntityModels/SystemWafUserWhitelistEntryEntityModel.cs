using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels
{
    public class SystemWafUserWhitelistEntryEntityModel
    {
        /// <summary>
        /// The unique identifier for the User Whitelist Entry.
        /// </summary>
        [Key]
        public required Guid Id { get; set; }
        /// <summary>
        /// The timestamp of when the User Whitelist Entry was created.
        /// </summary>
        public required DateTime CreatedAtUtc { get; set; }
        /// <summary>
        /// The unique identifier of the user who created the User Whitelist Entry.
        /// </summary>
        public Guid? CreatedBy { get; set; }





        /// <summary>
        /// The User to be Whitelisted.
        /// </summary>
        public required Guid UserId { get; set; }
        /// <summary>
        /// A reason for this User to be Whitelisted.
        /// </summary>
        public required string JustificationForWhitelist { get; set; }

        /// <summary>
        /// Whether the User Whitelist is permanent (i.e. does not expire and has to be manually removed) or temporary (i.e. expires at a certain date and time).
        /// </summary>
        public required bool IsPermanent { get; set; }





        /// <summary>
        /// When the User Whitelist expires, if it is not permanent.
        /// This MUST be null if the User Whitelist is permanent.
        /// </summary>
        public DateTime? ExpiresAtUtc { get; set; }

        /// <summary>
        /// For a temporary Whitelist, this denotes when the whitelist was lifted (i.e. when the User Whitelist became inactive).
        /// This MUST be null if the User Whitelist is permanent.
        /// This MUST be null if the User Whitelist is temporary and has not yet been lifted.
        /// </summary>
        public DateTime? UnwhitelistedAtUtc { get; set; }

        /// <summary>
        /// The unique identifier of the User who lifted the Whitelist, if applicable.
        /// This MUST be null if the User Whitelist is permanent.
        /// This MUST be null if the User Whitelist is temporary and has not yet been lifted.
        /// </summary>
        public Guid? UnwhitelistedBy { get; set; }

        /// <summary>
        /// A reason for why the User was Whitelisted.
        /// </summary>
        public string? JustificationForUnwhitelist { get; set; }





        public UserEntityModel? CreatedByUser { get; set; }
    }
}
