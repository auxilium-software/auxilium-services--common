using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TenantLifecycleStateEnum
    {
        /// <summary>
        /// This Tenant is running fine and dandy.
        /// </summary>
        [JsonPropertyName("active")]
        Active,

        /// <summary>
        /// This Tenant is in the grace period before destruction.
        /// </summary>
        [JsonPropertyName("suspended")]
        Suspended,

        /// <summary>
        /// This Tenant is next in line to be destroyed.
        /// </summary>
        [JsonPropertyName("awaiting_immediate_destruction")]
        AwaitingImmediateDestruction,

        /// <summary>
        /// The Task Runner service is currently going through and destroying data to do with this Tenant.
        /// </summary>
        [JsonPropertyName("destruction_in_progress")]
        DestructionInProgress,

        /// <summary>
        /// This record is empty, doesn't do anything, needs cleaning up.
        /// </summary>
        [JsonPropertyName("destroyed")]
        Destroyed,
    }
}
