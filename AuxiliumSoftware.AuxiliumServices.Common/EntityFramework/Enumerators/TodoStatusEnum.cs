using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TodoStatusEnum
    {
        [JsonPropertyName("NeedsAction")]
        NeedsAction,

        [JsonPropertyName("InProgress")]
        InProgress,

        [JsonPropertyName("Completed")]
        Completed,

        [JsonPropertyName("Cancelled")]
        Cancelled,
    }
}
