using System.ComponentModel.DataAnnotations;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels
{
    public class CaseWorkerEntityModel
    {
        /// <summary>
        /// The unique identifier for the Case Worker Assignment.
        /// </summary>
        [Key]
        public required Guid Id { get; set; }
        /// <summary>
        /// The timestamp of when the Case Worker Assignment was created.
        /// </summary>
        public required DateTime CreatedAtUtc { get; set; }
        /// <summary>
        /// The unique identifier of the User who created the Case Worker Assignment.
        /// </summary>
        public Guid? CreatedByUserId { get; set; }



        /// <summary>
        /// The unique identifier of the Case the Worker Assignment is for.
        /// </summary>
        public required Guid CaseId { get; set; }
        /// <summary>
        /// The unique identifier of the User Assigned to the Case.
        /// </summary>
        public required Guid UserId { get; set; }



        /// <summary>
        /// The User who created the Case Worker Assignment.
        /// </summary>
        public UserEntityModel? CreatedByUser { get; set; }
        /// <summary>
        /// The Case the Case Worker Assignment is for.
        /// </summary>
        public CaseEntityModel? Case { get; set; }
        /// <summary>
        /// The User the Case Worker Assignment is for.
        /// </summary>
        public UserEntityModel? User { get; set; }
    }
}
