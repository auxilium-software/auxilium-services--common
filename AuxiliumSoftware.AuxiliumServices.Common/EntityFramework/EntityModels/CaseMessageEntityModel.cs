using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels
{
    public class CaseMessageEntityModel : MutableTenantScopedEntityModel
    {
        /// <summary>
        /// The unique identifier of the Case the Case Message is for.
        /// </summary>
        public required Guid CaseId { get; set; }
        /// <summary>
        /// The unique identifier of the User who sent the Case Message.
        /// </summary>
        public required Guid SenderUserId { get; set; }
        /// <summary>
        /// The subject of the Case Message.
        /// </summary>
        public required string Subject { get; set; }
        /// <summary>
        /// The content/body of the Case Message.
        /// </summary>
        public required string Content { get; set; }
        /// <summary>
        /// Whether the Case Message should be treated as urgent.
        /// </summary>
        public required bool IsUrgent { get; set; }





        /// <summary>
        /// The Case the Case Message belongs to.
        /// </summary>
        public CaseEntityModel? Case { get; set; }
        /// <summary>
        /// The User who sent the Case Message.
        /// </summary>
        public UserEntityModel? Sender { get; set; }





        /// <summary>
        /// Read receipts for the Case Message.
        /// </summary>
        public ICollection<LogCaseMessageReadByEventEntityModel>? ReadBy { get; set; }
    }
}
