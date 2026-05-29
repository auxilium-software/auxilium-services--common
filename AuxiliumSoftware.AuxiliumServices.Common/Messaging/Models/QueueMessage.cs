using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.Common.Messaging.Models
{
    public abstract class QueueMessage
    {
        public Guid MessageId { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CorrelationId { get; set; }

        [JsonIgnore]
        public abstract string RoutingKey { get; }
    }
}
