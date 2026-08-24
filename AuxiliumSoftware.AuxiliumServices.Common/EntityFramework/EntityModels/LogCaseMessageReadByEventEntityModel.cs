using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels
{
    public class LogCaseMessageReadByEventEntityModel : TenantScopedEntityModel
    {
        /// <summary>
        /// The unique identifier of the User who read the Message.
        /// </summary>
        public Guid CreatedByUserId { get; set; }





        /// <summary>
        /// The unique identifier for the Message that has been read.
        /// </summary>
        public required Guid MessageId { get; set; }





        /// <summary>
        /// The User who read the Message.
        /// </summary>
        public UserEntityModel? CreatedByUser { get; set; }
        /// <summary>
        /// The Message that was read.
        /// </summary>
        public CaseMessageEntityModel? Message { get; set; }
    }
}
