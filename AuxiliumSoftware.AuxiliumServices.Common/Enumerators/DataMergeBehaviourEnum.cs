using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.Common.Enumerators
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum DataMergeBehaviourEnum
    {
        [JsonPropertyName("Repoint")]
        Repoint,

        [JsonPropertyName("Preserve")]
        Preserve,

        [JsonPropertyName("Discard")]
        Discard,
    }
}
