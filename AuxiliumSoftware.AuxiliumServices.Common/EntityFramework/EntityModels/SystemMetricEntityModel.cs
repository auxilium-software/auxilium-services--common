using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels
{
    public class SystemMetricEntityModel
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
        public required SystemMetricKeyEnum MetricKey { get; set; }

        /// <summary>
        /// </summary>
        public required float MetricValue { get; set; }
    }
}
