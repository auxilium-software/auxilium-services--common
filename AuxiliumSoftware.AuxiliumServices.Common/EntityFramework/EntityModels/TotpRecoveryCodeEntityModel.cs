using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels
{
    public class TotpRecoveryCodeEntityModel
    {
        /// <summary>
        /// The unique identifier for the TOTP Recovery Code.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// The timestamp of when the TOTP Recovery Code was created.
        /// </summary>
        public required DateTime CreatedAt { get; set; }
        /// <summary>
        /// The unique identifier of the User who created the TOTP Recovery Code (this is who the TOTP Recovery Code belongs to).
        /// </summary>
        public required Guid CreatedBy { get; set; }




        /// <summary>
        /// SHA256 hash of the plaintext TOTP Recovery Code (MUST be in lowercase hex).
        /// </summary>
        public required string CodeHash { get; set; }

        /// <summary>
        /// Whether the TOTP Recovery Code has been consumed.
        /// </summary>
        public required bool IsUsed { get; set; }

        /// <summary>
        /// When the TOTP Recovery Code has been used.
        /// Null if unused.
        /// </summary>
        public DateTime? UsedAt { get; set; }




        /// <summary>
        /// The User who created the TOTP Recovery Code (this is who the TOTP Recovery Code belongs to).
        /// </summary>
        public UserEntityModel? CreatedByUser { get; set; }
    }
}
