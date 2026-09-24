using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum DataMergeRowActionEnum
    {
        /// <summary>
        /// The row now references the survivor instead of the merged record.
        /// </summary>
        [JsonPropertyName("Moved")]
        Moved,

        /// <summary>
        /// The row was deleted.
        /// </summary>
        [JsonPropertyName("Discarded")]
        Discarded,
    }
}
