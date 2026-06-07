using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels
{
    public class LogSystemMessageQueueSentEmailEntityModel
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
        public required DateTime MessageCreatedAt { get; set; }
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
        public required string EmailRecipientAddress { get; set; }
        /// <summary>
        /// </summary>
        public required string EmailRecipientName { get; set; }
        /// <summary>
        /// </summary>
        public required string EmailLanguage { get; set; }
        /// <summary>
        /// </summary>
        public required string EmailTemplate { get; set; }
        /// <summary>
        /// </summary>
        public required string EmailSubject { get; set; }
        /// <summary>
        /// </summary>
        public required string EmailBodyHtml { get; set; }
        /// <summary>
        /// </summary>
        public required string EmailBodyTxt { get; set; }
    }
}
