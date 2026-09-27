using AuxiliumSoftware.AuxiliumServices.Common.Enumerators;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.Common.DataTransferObjects
{
    public class DataMergeReferenceSummaryDTO
    {
        /// <summary>
        /// The referencing entity, e.g. "CaseTimelineEntry".
        /// </summary>
        [JsonPropertyName("entity")]
        public required string Entity { get; init; }

        /// <summary>
        /// The referencing property, e.g. "CaseId".
        /// </summary>
        [JsonPropertyName("property")]
        public required string Property { get; init; }

        [JsonPropertyName("behaviour")]
        public required DataMergeBehaviourEnum Behaviour { get; init; }

        [JsonPropertyName("rowsToMove")]
        public required long RowsToMove { get; init; }

        /// <summary>
        /// Rows that will be deleted: collisions with the survivor under "repoint", everything under "discard".
        /// </summary>
        [JsonPropertyName("rowsToDiscard")]
        public required long RowsToDiscard { get; init; }

        [JsonPropertyName("rowsKeptOnDuplicate")]
        public required long RowsKeptOnDuplicate { get; init; }
    }
}
