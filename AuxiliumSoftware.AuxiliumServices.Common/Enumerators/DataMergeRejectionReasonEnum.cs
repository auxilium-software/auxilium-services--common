using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.Common.Enumerators
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum DataMergeRejectionReasonEnum
    {
        [JsonPropertyName("NotFound")]
        NotFound,

        [JsonPropertyName("Invalid")]
        Invalid,

        [JsonPropertyName("Stale")]
        Stale,
    }
}
