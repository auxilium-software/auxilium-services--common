using System.ComponentModel.DataAnnotations;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels
{
    public class RefreshTokenEntityModel
    {
        /// <summary>
        /// The unique identifier for the Refresh Token.
        /// </summary>
        [Key]
        public required Guid Id { get; set; }
        /// <summary>
        /// The timestamp of when the Refresh Token was created.
        /// </summary>
        public required DateTime CreatedAtUtc { get; set; }
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
