using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.Common.Metrics.Common
{
    internal sealed record DiskRow
    {
        [JsonPropertyName("t")] public DateTime T { get; init; }
        [JsonPropertyName("v")] public double V { get; init; }
        [JsonPropertyName("l")] public string? L { get; init; }
    }
}
