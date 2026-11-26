using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.Common.DataTransferObjects
{
    public class AdditionalPropertyDefinitionSeedResultDTO
    {
        [JsonPropertyName("createdDefinitionCount")]
        public int CreatedDefinitionCount { get; set; }

        [JsonPropertyName("addedAliasCount")]
        public int AddedAliasCount { get; set; }

        [JsonPropertyName("skippedAliases")]
        public List<string> SkippedAliases { get; set; } = new();

        [JsonPropertyName("linkedPropertyCount")]
        public int LinkedPropertyCount { get; set; }
    }
}
