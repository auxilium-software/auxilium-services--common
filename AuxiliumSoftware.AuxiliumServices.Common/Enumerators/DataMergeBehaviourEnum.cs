using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.Common.Enumerators
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum DataMergeBehaviourEnum
    {
        /// <summary>
        /// This should be the default.
        /// The row is moved onto the survivor data object.
        /// Rows that would break a unique index on the survivor (eg the same client already on the surviving case) are removed and "snapshotted" in the merge record.
        /// </summary>
        [JsonPropertyName("Repoint")]
        Repoint,

        /// <summary>
        /// The row is left pointing at the tombstone.
        /// This should only really be used for immutable data such as audit logs.
        /// </summary>
        [JsonPropertyName("Preserve")]
        Preserve,

        /// <summary>
        /// The row is deleted.
        /// Should only really be used for things that must not transfer, such as a duplicate user's refresh tokens.
        /// </summary>
        [JsonPropertyName("Discard")]
        Discard,
    }
}
