using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.Common.DataTransferObjects
{
    public class UnlinkedAdditionalPropertyNameDTO
    {
        [JsonPropertyName("normalisedName")]
        public required string NormalisedName { get; set; }

        [JsonPropertyName("displayNames")]
        public required List<string> DisplayNames { get; set; }

        [JsonPropertyName("propertyCount")]
        public required int PropertyCount { get; set; }
    }
}
