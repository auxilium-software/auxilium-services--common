using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels
{
    public class TotpRecoveryCodeEntityModel : TenantScopedEntityModel
    {
        /// <summary>
        /// The unique identifier of the User who created the TOTP Recovery Code (this is who the TOTP Recovery Code belongs to).
        /// </summary>
        public required Guid CreatedByUserId { get; set; }




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
        public DateTime? UsedAtUtc { get; set; }





        /// <summary>
        /// The User who created the TOTP Recovery Code (this is who the TOTP Recovery Code belongs to).
        /// </summary>
        public UserEntityModel? CreatedByUser { get; set; }
    }
}
