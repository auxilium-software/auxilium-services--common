using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels
{
    public class CaseClientEntityModel : TenantScopedEntityModel
    {
        /// <summary>
        /// The unique identifier of the User who created the Case Client Assignment.
        /// </summary>
        public Guid? CreatedByUserId { get; set; }



        /// <summary>
        /// The unique identifier of the Case the Client Assignment is for.
        /// </summary>
        public required Guid CaseId { get; set; }
        /// <summary>
        /// The unique identifier of the User Assigned to the Case.
        /// </summary>
        public required Guid UserId { get; set; }





        /// <summary>
        /// The User who created the Case Client Assignment.
        /// </summary>
        public UserEntityModel? CreatedByUser { get; set; }
        /// <summary>
        /// The Case the Case Client Assignment is for.
        /// </summary>
        public CaseEntityModel? Case { get; set; }
        /// <summary>
        /// The User the Case Client Assignment is for.
        /// </summary>
        public UserEntityModel? User { get; set; }
    }
}
