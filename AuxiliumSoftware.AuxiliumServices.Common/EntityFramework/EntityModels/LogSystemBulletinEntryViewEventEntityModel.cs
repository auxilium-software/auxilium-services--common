using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels
{
    public class LogSystemBulletinEntryViewEventEntityModel
    {
        /// <summary>
        /// The unique identifier for the System Bulletin View Log Entry.
        /// </summary>
        [Key]
        public required Guid Id { get; set; }
        /// <summary>
        /// The timestamp of when the System Bulletin was viewed.
        /// </summary>
        public required DateTime CreatedAtUtc { get; set; }
        /// <summary>
        /// The unique identifier of the User who viewed the System Bulletin.
        /// </summary>
        public Guid CreatedBy { get; set; }





        /// <summary>
        /// The unique identifier of the Bulletin that was viewed.
        /// </summary>
        public required Guid SystemBulletinId { get; set; }





        /// <summary>
        /// The User who viewed the System Bulletin.
        /// </summary>
        public UserEntityModel? CreatedByUser { get; set; }
        /// <summary>
        /// The System Bulletin that was viewed.
        /// </summary>
        public SystemBulletinEntryEntityModel? SystemBulletin { get; set; }
    }
}
