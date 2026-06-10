using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels
{
    public class DataEnumeratorValueTranslationEntityModel
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
        public Guid? CreatedBy { get; set; }
        /// <summary>
        /// </summary>
        public DateTime? LastUpdatedAtUtc { get; set; }
        /// <summary>
        /// </summary>
        public Guid? LastUpdatedBy { get; set; }





        /// <summary>
        /// </summary>
        public required Guid DataEnumeratorValueId { get; set; }





        /// <summary>
        /// </summary>
        public required string LanguageCode { get; set; }
        /// <summary>
        /// </summary>
        public required string Translation { get; set; }




        /// <summary>
        /// </summary>
        public UserEntityModel? CreatedByUser { get; set; }
        /// <summary>
        /// </summary>
        public UserEntityModel? LastUpdatedByUser { get; set; }
        /// <summary>
        /// </summary>
        public DataEnumeratorValueEntityModel? EnumValue { get; set; }
    }
}
