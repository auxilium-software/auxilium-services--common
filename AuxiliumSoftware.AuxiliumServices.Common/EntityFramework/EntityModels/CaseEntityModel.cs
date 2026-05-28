using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators;
using System.ComponentModel.DataAnnotations;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels
{
    public class CaseEntityModel
    {
        /// <summary>
        /// The unique identifier for the Case.
        /// </summary>
        [Key]
        public required Guid Id { get; set; }
        /// <summary>
        /// The timestamp of when the Case was created.
        /// </summary>
        public required DateTime CreatedAt { get; set; }
        /// <summary>
        /// The unique identifier of the User who created the Case.
        /// </summary>
        public Guid? CreatedBy { get; set; }
        /// <summary>
        /// The timestamp of when the Case was last updated.
        /// </summary>
        public DateTime? LastUpdatedAt { get; set; }
        /// <summary>
        /// The unique identifier of the User who last updated the Case.
        /// </summary>
        public Guid? LastUpdatedBy { get; set; }




        /// <summary>
        /// The title of the Case.
        /// </summary>
        public required string Title { get; set; }
        /// <summary>
        /// A description of the Case.
        /// </summary>
        public required string Description { get; set; }
        /// <summary>
        /// The Sensitivity Level of the Case.
        /// </summary>
        public required CaseSensitivityEnum Sensitivity { get; set; }
        /// <summary>
        /// The current Status of the Case.
        /// </summary>
        public required CaseStatusEnum Status { get; set; }




        /// <summary>
        /// The User who created the Case.
        /// </summary>
        public UserEntityModel? CreatedByUser { get; set; }
        /// <summary>
        /// The User who last updated the Case.
        /// </summary>
        public UserEntityModel? LastUpdatedByUser { get; set; }





        /// <summary>
        /// The Users that are assigned to the Case as a Case Worker.
        /// </summary>
        public ICollection<CaseWorkerEntityModel>? Workers { get; set; }
        /// <summary>
        /// The Users that are assigned to the Case as a Client.
        /// </summary>
        public ICollection<CaseClientEntityModel>? Clients { get; set; }
        /// <summary>
        /// Any Additional Properties created on the Case.
        /// </summary>
        public ICollection<CaseAdditionalPropertyEntityModel>? AdditionalProperties { get; set; }
        /// <summary>
        /// Any Messages created on the Case.
        /// </summary>
        public ICollection<CaseMessageEntityModel>? Messages { get; set; }
        /// <summary>
        /// Any Files created on the Case.
        /// </summary>
        public ICollection<CaseFileEntityModel>? Files { get; set; }
        /// <summary>
        /// Any Todos created on the Case.
        /// </summary>
        public ICollection<CaseTodoEntityModel>? Todos { get; set; }
        /// <summary>
        /// A Log of Modification Events related to the Case.
        /// </summary>
        public ICollection<LogCaseModificationEventEntityModel>? EventLog { get; set; }
    }
}
