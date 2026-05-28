using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels
{
    public class PasswordSetTokenEntityModel
    {
        /// <summary>
        /// The unique identifier for the Password Set Token.
        /// </summary>
        [Key]
        public required Guid Id { get; set; }
        /// <summary>
        /// The timestamp of when the Password Set Token was created.
        /// </summary>
        public required DateTime CreatedAt { get; set; }
        /// <summary>
        /// The unique identifier of the User who issued the Password Set Token.
        /// </summary>
        public required Guid CreatedBy { get; set; }
        /// <summary>
        /// The unique identifier of the User the Password Set Token is for.
        /// </summary>
        public required Guid UserId { get; set; }
        /// <summary>
        /// A hash of the Password Set Token.
        /// </summary>
        public required string TokenHash { get; set; }
        /// <summary>
        /// The expiration datetime of the Password Set Token.
        /// </summary>
        public required DateTime ExpiresAt { get; set; }
        /// <summary>
        /// When the Password Set Token was consumed. Null if unused.
        /// </summary>
        public DateTime? UsedAt { get; set; } = null;
        /// <summary>
        /// The reason this Password Set Token was issued.
        /// </summary>
        public required PasswordSetTokenReasonEnum Reason { get; set; }




        /// <summary>
        /// The User who issued the Password Set Token.
        /// </summary>
        public UserEntityModel? CreatedByUser { get; set; }
    }
}
