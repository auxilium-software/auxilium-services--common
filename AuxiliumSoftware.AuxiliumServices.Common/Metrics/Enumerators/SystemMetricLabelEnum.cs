using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.Common.Metrics.Enumerators
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SystemMetricLabelEnum
    {
        // rabbitmq queues
        [JsonPropertyName("rabbitmq.queue.notifications")]
        RabbitMq_Notifications,
        [JsonPropertyName("rabbitmq.queue.dead-letter")]
        RabbitMq_DeadLetter,

        // storage roots
        [JsonPropertyName("storage.form-data")]
        Storage_FormData,
        [JsonPropertyName("storage.aux-lfs")]
        Storage_AuxLfs,
    }
}
