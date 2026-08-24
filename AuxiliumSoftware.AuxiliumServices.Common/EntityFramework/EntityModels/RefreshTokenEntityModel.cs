using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels
{
    public class RefreshTokenEntityModel : TenantScopedEntityModel
    {
        /// <summary>
        /// The unique identifier of the User who created the Refresh Token.
        /// </summary>
        public Guid? CreatedByUserId { get; set; }





        /// <summary>
        /// A hash of the Refresh Token.
        /// </summary>
        public required string TokenHash { get; set; }
        /// <summary>
        /// The expiration datetime of the Refresh Token.
        /// </summary>
        public required DateTime ExpiresAtUtc { get; set; }





        /// <summary>
        /// The User that created the Refresh Token.
        /// </summary>
        public UserEntityModel? CreatedByUser { get; set; }
    }
}
