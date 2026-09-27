using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.Common.DataTransferObjects
{
    public class DataMergePreviewDTO
    {
        [JsonPropertyName("survivorId")]
        public required Guid SurvivorId { get; init; }

        [JsonPropertyName("duplicateId")]
        public required Guid DuplicateId { get; init; }

        /// <summary>
        /// Must be sent back unchanged when confirming.
        /// If either record changes in between, the merge is refused.
        /// </summary>
        [JsonPropertyName("fingerprint")]
        public required string Fingerprint { get; init; }

        /// <summary>
        /// Every resolvable field, whether or not the two sides differ, so the UI can show a full comparison.
        /// </summary>
        [JsonPropertyName("fields")]
        public required List<DataMergeFieldConflictDTO> Fields { get; init; }

        /// <summary>
        /// Only tables with at least one affected row are listed.
        /// </summary>
        [JsonPropertyName("references")]
        public required List<DataMergeReferenceSummaryDTO> References { get; init; }
    }
}
