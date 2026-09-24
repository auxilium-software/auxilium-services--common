using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TenantLifecycleStatusEnum
    {
        /// <summary>
        /// This Tenant is running fine and dandy.
        /// </summary>
        [JsonPropertyName("Active")]
        Active,

        /// <summary>
        /// This Tenant is in the grace period before destruction.
        /// </summary>
        [JsonPropertyName("Suspended")]
        Suspended,

        /// <summary>
        /// This Tenant is next in line to be destroyed.
        /// </summary>
        [JsonPropertyName("AwaitingImmediateDestruction")]
        AwaitingImmediateDestruction,

        /// <summary>
        /// The Task Runner service is currently going through and destroying data to do with this Tenant.
        /// </summary>
        [JsonPropertyName("DestructionInProgress")]
        DestructionInProgress,

        /// <summary>
        /// This record is empty, doesn't do anything, needs cleaning up.
        /// </summary>
        [JsonPropertyName("Destroyed")]
        Destroyed,
    }
}
