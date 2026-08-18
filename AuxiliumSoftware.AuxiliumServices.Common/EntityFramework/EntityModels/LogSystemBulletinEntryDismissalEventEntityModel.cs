using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels
{
    public class LogSystemBulletinEntryDismissalEventEntityModel : IMandatoryFieldsEntityModel
    {
        /// <summary>
        /// The unique identifier of the User who dismissed the System Bulletin.
        /// </summary>
        public Guid CreatedByUserId { get; set; }





        /// <summary>
        /// The unique identifier of the Bulletin that was dismissed.
        /// </summary>
        public required Guid SystemBulletinId { get; set; }





        /// <summary>
        /// The User who dismissed the System Bulletin.
        /// </summary>
        public UserEntityModel? CreatedByUser { get; set; }
        /// <summary>
        /// The System Bulletin that was dismissed.
        /// </summary>
        public SystemBulletinEntryEntityModel? SystemBulletin { get; set; }
    }
}
