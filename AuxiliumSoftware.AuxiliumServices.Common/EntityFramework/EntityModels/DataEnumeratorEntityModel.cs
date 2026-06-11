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
        public Guid? CreatedBy { get; set; }
        /// <summary>
        /// </summary>
        public DateTime? LastUpdatedAtUtc { get; set; }
        /// <summary>
        /// </summary>
        public Guid? LastUpdatedBy { get; set; }





        /// <summary>
        /// </summary>
        public required string Name { get; set; }
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
    }
}
