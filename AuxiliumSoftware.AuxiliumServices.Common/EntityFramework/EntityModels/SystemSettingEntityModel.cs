using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels
{
    public class SystemSettingEntityModel
    {
        /// <summary>
        /// The unique identifier for the Configuration Value.
        /// </summary>
        [Key]
        public required Guid Id { get; set; }
        /// <summary>
        /// The timestamp of when the Configuration Value was created.
        /// </summary>
        public required DateTime CreatedAtUtc { get; set; }
        /// <summary>
        /// The unique identifier of the user who created the Configuration Value.
        /// </summary>
        public Guid? CreatedBy { get; set; }





        /// <summary>
        /// The key of the Configuration Value.
        /// </summary>
        public required SystemSettingKeyEnum ConfigKey { get; set; }
        /// <summary>
        /// The data type of the Configuration Value.
        /// </summary>
        public required SystemSettingValueTypeEnum ValueType { get; set; }
        /// <summary>
        /// The value of the Configuration Value, MUST be stored as a JSON string.
        /// </summary>
        public required string ConfigValue { get; set; }





        /// <summary>
        /// The reason for modifying the Configuration Value, required for audit logging purposes.
        /// </summary>
        public required string ReasonForModification { get; set; }





        /// <summary>
        /// The User who created the Configuration Value.
        /// </summary>
        public UserEntityModel? CreatedByUser { get; set; }
    }
}
