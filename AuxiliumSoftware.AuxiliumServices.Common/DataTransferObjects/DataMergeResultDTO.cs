using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.Common.DataTransferObjects
{
    public class DataMergeResultDTO
    {
        [JsonPropertyName("mergeEventId")]
        public required Guid MergeEventId { get; init; }

        [JsonPropertyName("survivorId")]
        public required Guid SurvivorId { get; init; }

        [JsonPropertyName("duplicateId")]
        public required Guid DuplicateId { get; init; }

        [JsonPropertyName("rowsMoved")]
        public required long RowsMoved { get; init; }

        [JsonPropertyName("rowsDiscarded")]
        public required long RowsDiscarded { get; init; }
    }
}
