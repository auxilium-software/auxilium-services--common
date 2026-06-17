using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators;
using System.ComponentModel.DataAnnotations;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels
{
    public class CaseTodoEntityModel
    {
        /// <summary>
        /// The unique identifier for the Case Todo Entry.
        /// </summary>
        [Key]
        public required Guid Id { get; set; }
        /// <summary>
        /// The timestamp of when the Case Todo Entry was created.
        /// </summary>
        public required DateTime CreatedAtUtc { get; set; }
        /// <summary>
        /// The unique identifier of the User who created the Case Todo Entry.
        /// </summary>
        public Guid? CreatedByUserId { get; set; }
        /// <summary>
        /// The timestamp of when the Case Todo Entry was last updated.
        /// </summary>
        public DateTime? LastUpdatedAtUtc { get; set; }
        /// <summary>
        /// The unique identifier of the User who last updated the Case Todo Entry.
        /// </summary>
        public Guid? LastUpdatedByUserId { get; set; }





        /// <summary>
        /// The unique identifier for the case the todo is for.
        /// </summary>
        public required Guid CaseId { get; set; }





        /// <summary>
        /// The summary/title of the Case Todo Entry.
        /// </summary>
        public required string Summary { get; set; }
        /// <summary>
        /// The detailed description of the Case Todo Entry.
        /// </summary>
        public required string Description { get; set; }
        /// <summary>
        /// The current status of the Case Todo Entry.
        /// </summary>
        public required TodoStatusEnum Status { get; set; }
        /// <summary>
        /// The priority level of the Case Todo Entry.
        /// </summary>
        public required TodoPriorityEnum Priority { get; set; }
        /// <summary>
        /// An optional due date for the Case Todo Entry.
        /// </summary>
        public required DateTime? DueDate { get; set; }
        /// <summary>
        /// An optional unique identifier of the User the Case Todo Entry is assigned to.
        /// </summary>
        public required Guid? AssignedToUserId { get; set; }
        /// <summary>
        /// An optional reminder timestamp for the Case Todo Entry.
        /// </summary>
        public required DateTime? ReminderUtc { get; set; }
        /// <summary>
        /// The timestamp of when the Case Todo Entry was completed.
        /// </summary>
        public DateTime? CompletedAtUtc { get; set; }
        /// <summary>
        /// The unique identifier of the User who completed the Case Todo Entry.
        /// </summary>
        public Guid? CompletedByUserId { get; set; }
        /// <summary>
        /// An optional note added upon completion of the Case Todo Entry.
        /// </summary>
        public string? CompletionNote { get; set; }





        /// <summary>
        /// The User that created the Case Todo Entry.
        /// </summary>
        public UserEntityModel? CreatedByUser { get; set; }
        /// <summary>
        /// The User that last updated the Case Todo Entry.
        /// </summary>
        public UserEntityModel? LastUpdatedByUser { get; set; }





        /// <summary>
        /// The case the todo belongs to.
        /// </summary>
        public CaseEntityModel? Case { get; set; }
        /// <summary>
        /// The User the todo is assigned to.
        /// </summary>
        public UserEntityModel? AssignedToUser { get; set; }
        /// <summary>
        /// The User who completed the todo.
        /// </summary>
        public UserEntityModel? CompletedByUser { get; set; }
    }
}
