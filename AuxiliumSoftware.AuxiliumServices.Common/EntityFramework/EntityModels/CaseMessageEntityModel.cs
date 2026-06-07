using System.ComponentModel.DataAnnotations;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels
{
    public class CaseMessageEntityModel
    {
        /// <summary>
        /// The unique identifier for the case Message.
        /// </summary>
        [Key]
        public required Guid Id { get; set; }
        /// <summary>
        /// The timestamp of when the Case Message was created.
        /// </summary>
        public required DateTime CreatedAtUtc { get; set; }
        /// <summary>
        /// The unique identifier of the User who created the Case Message.
        /// </summary>
        public Guid? CreatedBy { get; set; }
        /// <summary>
        /// The timestamp of when the Case Message was last updated.
        /// </summary>
        public DateTime? LastUpdatedAtUtc { get; set; }
        /// <summary>
        /// The unique identifier of the User who last updated the Case Message.
        /// </summary>
        public Guid? LastUpdatedBy { get; set; }





        /// <summary>
        /// The unique identifier of the Case the Case Message is for.
        /// </summary>
        public required Guid CaseId { get; set; }
        /// <summary>
        /// The unique identifier of the User who sent the Case Message.
        /// </summary>
        public required Guid SenderId { get; set; }
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
        /// The User who created the Case Message.
        /// </summary>
        public UserEntityModel? CreatedByUser { get; set; }
        /// <summary>
        /// The User who last updated the Case Message.
        /// </summary>
        public UserEntityModel? LastUpdatedByUser { get; set; }





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
