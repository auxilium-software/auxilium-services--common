using AuxiliumSoftware.AuxiliumServices.Common.Enumerators;
using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.Common.DataTransferObjects
{
    public class DataMergeFieldConflictDTO
    {
        /// <summary>
        /// The key to use in the confirm request's field choices, e.g. "title".
        /// </summary>
        [JsonPropertyName("field")]
        public required string Field { get; init; }

        [JsonPropertyName("survivorValue")]
        public string? SurvivorValue { get; init; }

        [JsonPropertyName("duplicateValue")]
        public string? DuplicateValue { get; init; }

        [JsonPropertyName("differs")]
        public required bool Differs { get; init; }

        [JsonPropertyName("canCombine")]
        public required bool CanCombine { get; init; }

        [JsonPropertyName("suggestedSource")]
        public required DataMergeSourceEnum SuggestedSource { get; init; }
    }
}
