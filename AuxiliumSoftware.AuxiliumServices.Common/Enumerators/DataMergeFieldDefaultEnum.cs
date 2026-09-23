using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.Common.Enumerators
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum DataMergeFieldDefaultEnum
    {
        [JsonPropertyName("PreferSurvivorUnlessEmpty")]
        PreferSurvivorUnlessEmpty,

        [JsonPropertyName("PreferHigherValue")]
        PreferHigherValue,

        [JsonPropertyName("PreferLowerValue")]
        PreferLowerValue,
    }
}
