using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels
{
    public class DataEnumeratorEntityModel
    {
        /// <summary>
        /// </summary>
        [Key]
        public required Guid Id { get; set; }
        /// <summary>
        /// </summary>
        public required DateTime CreatedAtUtc { get; set; }
        /// <summary>
        /// </summary>
        public Guid? CreatedByUserId { get; set; }
        /// <summary>
        /// </summary>
        public DateTime? LastUpdatedAtUtc { get; set; }
        /// <summary>
        /// </summary>
        public Guid? LastUpdatedByUserId { get; set; }





        /// <summary>
        /// </summary>
        public required string CanonicalName { get; set; }
        /// <summary>
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// </summary>
        public required bool IsActive { get; set; }




        /// <summary>
        /// </summary>
        public UserEntityModel? CreatedByUser { get; set; }
        /// <summary>
        /// </summary>
        public UserEntityModel? LastUpdatedByUser { get; set; }
        /// <summary>
        /// </summary>
        public ICollection<DataEnumeratorValueEntityModel>? EnumeratorValues { get; set; }
        /// <summary>
        /// </summary>
        public ICollection<DataEnumeratorTranslationEntityModel>? Translations { get; set; }
    }
}
