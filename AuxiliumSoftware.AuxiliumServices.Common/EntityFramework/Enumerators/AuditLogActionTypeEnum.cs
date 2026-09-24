using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AuditLogActionTypeEnum
    {
        [JsonPropertyName("Creation")]
        Creation,
        [JsonPropertyName("Modification")]
        Modification,
        [JsonPropertyName("Deletion")]
        Deletion,

        [JsonPropertyName("Assignment")]
        Assignment,
        [JsonPropertyName("Unassignment")]
        Unassignment,

        [JsonPropertyName("View")]
        View,

        [JsonPropertyName("Send")]
        Send,

        [JsonPropertyName("Upload")]
        Upload,

        [JsonPropertyName("Merge")]
        Merge,
    }
}
