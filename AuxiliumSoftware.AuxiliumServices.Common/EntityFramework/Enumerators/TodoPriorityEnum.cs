using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TodoPriorityEnum
    {
        [JsonPropertyName("Low")]
        Low,

        [JsonPropertyName("Medium")]
        Medium,

        [JsonPropertyName("High")]
        High,

        [JsonPropertyName("Urgent")]
        Urgent,
    }
}
