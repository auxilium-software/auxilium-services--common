using System.ComponentModel.DataAnnotations;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels
{
    public class CaseFileEntityModel
    {
        /// <summary>
        /// The unique identifier for the Case File.
        /// </summary>
        [Key]
        public required Guid Id { get; set; }
        /// <summary>
        /// The timestamp of when the Case File was created.
        /// </summary>
        public required DateTime CreatedAtUtc { get; set; }
        /// <summary>
        /// The unique identifier of the User who created the Case File.
        /// </summary>
        public Guid? CreatedByUserId { get; set; }
        /// <summary>
        /// The timestamp of when the Case File was last updated.
        /// </summary>
        public DateTime? LastUpdatedAtUtc { get; set; }
        /// <summary>
        /// The unique identifier of the User who last updated the Case File.
        /// </summary>
        public Guid? LastUpdatedByUserId { get; set; }





        /// <summary>
        /// The unique identifier for the Case the File is for.
        /// </summary>
        public required Guid CaseId { get; set; }
        /// <summary>
        /// The original filename of the File.
        /// </summary>
        public required string Filename { get; set; }
        /// <summary>
        /// The MIME type of the File.
        /// </summary>
        /// <example>
        /// image/png
        /// </example>
        /// <example>
        /// application/pdf
        /// </example>
        public required string ContentType { get; set; }
        /// <summary>
        /// The size of the File in bytes.
        /// </summary>
        public required long Size { get; set; }
        /// <summary>
        /// A hash (checksum) of the File.
        /// </summary>
        public required string Hash { get; set; }
        /// <summary>
        /// The filepath (relative to that set in config) to the File in LFS.
        /// </summary>
        public required string LfsPath { get; set; }
        /// <summary>
        /// An optional description of the File the user can set.
        /// </summary>
        public required string Description { get; set; }





        /// <summary>
        /// The User who created the File.
        /// </summary>
        public UserEntityModel? CreatedByUser { get; set; }
        /// <summary>
        /// The User who last updated the File.
        /// </summary>
        public UserEntityModel? LastUpdatedByUser { get; set; }
        /// <summary>
        /// The Case the File is for.
        /// </summary>
        public CaseEntityModel? Case { get; set; }
    }
}
