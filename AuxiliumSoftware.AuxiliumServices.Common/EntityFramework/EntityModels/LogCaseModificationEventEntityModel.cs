using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels
{
    public class LogCaseModificationEventEntityModel
    {
        /// <summary>
        /// The unique identifier for the Additional Property.
        /// </summary>
        [Key]
        public required Guid Id { get; set; }
        /// <summary>
        /// The timestamp of when the Additional Property was created.
        /// </summary>
        public required DateTime CreatedAtUtc { get; set; }
        /// <summary>
        /// The unique identifier of the user who created the Additional Property.
        /// </summary>
        public required Guid CreatedBy { get; set; }





        /// <summary>
        /// The unique identifier of the Case the Additional Property is for.
        /// </summary>
        public required Guid CaseId { get; set; }





        /// <summary>
        /// The type of Entity that was Actioned on.
        /// </summary>
        /// <example>
        /// This could be the "case" itself, or a "case.message", or a "case.todo", etc.
        /// </example>
        public required CaseEntityTypeEnum EntityType { get; set; }

        /// <summary>
        /// The unique identifier of Entity that was Actioned on.
        /// </summary>
        public required Guid EntityId { get; set; }

        /// <summary>
        /// The type of Action that was performed.
        /// </summary>
        public required AuditLogActionTypeEnum Action { get; set; }

        /// <summary>
        /// The name of the Property that was modified (if Action == `Modified`).
        /// </summary>
        public string? PropertyName { get; set; }

        /// <summary>
        /// The previous value of the Property that was modified (if Action == `Modified`).
        /// </summary>
        public string? PreviousValue { get; set; }

        /// <summary>
        /// The new value of the Property that was modified (if Action == `Modified`).
        /// </summary>
        public string? NewValue { get; set; }





        /// <summary>
        /// The User who Actioned on the Case//Case related Entity
        /// </summary>
        public UserEntityModel? CreatedByUser { get; set; }
        /// <summary>
        /// The Case that was Actioned on.
        /// </summary>
        public CaseEntityModel? Case { get; set; }
    }
}
