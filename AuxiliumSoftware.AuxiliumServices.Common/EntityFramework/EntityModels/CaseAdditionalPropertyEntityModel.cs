using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Abstractions;
using System;
using System.ComponentModel.DataAnnotations;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels
{
    public class CaseAdditionalPropertyEntityModel : MutableTenantScopedEntityModel
    {
        /// <summary>
        /// The unique identifier of the Case the Additional Property is for.
        /// </summary>
        public required Guid CaseId { get; set; }
        /// <summary>
        /// This is the Original Name of the Additional Property (eg what the User entered into the text field in the GUI) (can have spaces and special characters).
        /// I.E. This is what the User has entered.
        /// </summary>
        public required string DisplayName { get; set; }
        /// <summary>
        /// The MIME type of the Additional Property.
        /// </summary>
        /// <example>
        /// text/plain
        /// </example>
        /// <example>
        /// application/json
        /// </example>
        public required string ContentType { get; set; }
        /// <summary>
        /// The actual content of the Additional Property.
        /// </summary>
        public required string Content { get; set; }





        /// <summary>
        /// The Case the Additional Property is for.
        /// </summary>
        public CaseEntityModel? Case { get; set; }
    }
}
