using System;
using System.ComponentModel.DataAnnotations;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels
{
    public class CaseAdditionalPropertyEntityModel
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
        public Guid? CreatedByUserId { get; set; }
        /// <summary>
        /// The timestamp of when the Additional Property was last updated.
        /// </summary>
        public DateTime? LastUpdatedAtUtc { get; set; }
        /// <summary>
        /// The unique identifier of the user who last updated the Additional Property.
        /// </summary>
        public Guid? LastUpdatedByUserId { get; set; }





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
        /// The User who created the Additional Property.
        /// </summary>
        public UserEntityModel? CreatedByUser { get; set; }
        /// <summary>
        /// The User who last updated the Additional Property.
        /// </summary>
        public UserEntityModel? LastUpdatedByUser { get; set; }
        /// <summary>
        /// The Case the Additional Property is for.
        /// </summary>
        public CaseEntityModel? Case { get; set; }
    }
}
