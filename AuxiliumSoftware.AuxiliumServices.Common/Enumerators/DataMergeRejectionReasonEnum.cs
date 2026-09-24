using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.Common.Enumerators
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum DataMergeRejectionReasonEnum
    {
        /// <summary>
        /// One or both records do not exist within the current Tenant.
        /// </summary>
        [JsonPropertyName("NotFound")]
        NotFound,

        /// <summary>
        /// The request itself is invalid: same record twice, already merged, an unknown field choice, and so on.
        /// </summary>
        [JsonPropertyName("Invalid")]
        Invalid,

        /// <summary>
        /// One of the records changed after the preview was generated.
        /// The administrator needs to review it again.
        /// </summary>
        [JsonPropertyName("Stale")]
        Stale,
    }
}
