using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels
{
    public class SystemWafIpBlacklistEntryEntityModel
    {
        /// <summary>
        /// The unique identifier for the IP Block.
        /// </summary>
        [Key]
        public required Guid Id { get; set; }

        /// <summary>
        /// The timestamp of when the IP Block was created.
        /// </summary>
        public required DateTime CreatedAtUtc { get; set; }

        /// <summary>
        /// The unique identifier of the User who created the IP Block.
        /// </summary>
        public Guid? CreatedBy { get; set; }





        /// <summary>
        /// The IP Address that is blacklisted.
        /// </summary>
        public required IPAddress IpAddress { get; set; }

        /// <summary>
        /// A reason for why the IP Address was blacklisted.
        /// </summary>
        public required string JustificationForBlacklist { get; set; }

        /// <summary>
        /// Whether the IP Address Block is permanent (i.e. does not expire and has to be manually removed) or temporary (i.e. expires at a certain date and time).
        /// </summary>
        public required bool IsPermanent { get; set; }





        /// <summary>
        /// When the IP Block expires, if it is not permanent.
        /// This MUST be null if the IP Block is permanent.
        /// </summary>
        public DateTime? ExpiresAtUtc { get; set; }

        /// <summary>
        /// For a temporary block, this denotes when the Blacklist Entry was lifted (i.e. when the IP Address Blacklist Entry became inactive).
        /// This MUST be null if the IP Address Blacklist is permanent.
        /// This MUST be null if the IP Address Blacklist is temporary and has not yet been lifted.
        /// </summary>
        public DateTime? UnblacklistedAtUtc { get; set; }

        /// <summary>
        /// The unique identifier of the User who lifted the block, if applicable.
        /// This MUST be null if the IP Address Block is permanent.
        /// This MUST be null if the IP Address Block is temporary and has not yet been lifted.
        /// </summary>
        public Guid? UnblacklistedBy { get; set; }

        /// <summary>
        /// A reason for why the IP Address was blacklisted.
        /// </summary>
        public string? JustificationForUnblacklist { get; set; }





        public UserEntityModel? CreatedByUser { get; set; }
        public UserEntityModel? UnblockedByUser { get; set; }
    }
}
