using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.Common.Enumerators
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum MetricCadence
    {
        [JsonPropertyName("Hourly")]
        Hourly,


        [JsonPropertyName("Minutely")]
        Minutely,
    }
}
