using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels
{
    public class LogSystemMessageQueueFailedActionEntityModel
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
        public required Guid MessageId { get; set; }
        /// <summary>
        /// </summary>
        public required DateTime MessageCreatedAtUtc { get; set; }
        /// <summary>
        /// </summary>
        public required string? MessageCorrelationId { get; set; }
        /// <summary>
        /// </summary>
        public required string MessageRoutingKey { get; set; }
        /// <summary>
        /// </summary>
        public required string MessageJson { get; set; }
        /// <summary>
        /// </summary>
        public required string ExceptionType { get; set; }
        /// <summary>
        /// </summary>
        public required string ExceptionMessage { get; set; }
        /// <summary>
        /// </summary>
        public required string ExceptionStackTrace { get; set; }
    }
}
