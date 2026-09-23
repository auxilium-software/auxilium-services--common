using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CaseSensitivityEnum
    {
        [JsonPropertyName("Public")]
        Public,

        [JsonPropertyName("Internal")]
        Internal,

        [JsonPropertyName("Confidential")]
        Confidential,

        [JsonPropertyName("Restricted")]
        Restricted
    }
}
