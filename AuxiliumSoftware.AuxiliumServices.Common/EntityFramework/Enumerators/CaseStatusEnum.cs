using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CaseStatusEnum
    {
        [JsonPropertyName("Staging")]
        Staging,

        [JsonPropertyName("Open")]
        Open,

        [JsonPropertyName("Closed")]
        Closed,

        [JsonPropertyName("Archived")]
        Archived
    }
}
