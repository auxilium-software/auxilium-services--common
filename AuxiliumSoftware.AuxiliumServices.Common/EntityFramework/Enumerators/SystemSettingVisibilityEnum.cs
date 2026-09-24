using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SystemSettingVisibilityEnum
    {
        [JsonPropertyName("Public")]
        Public,          // unauthenticated - logos, contact info, branding

        // [JsonPropertyName("authenticated")]
        // Authenticated,   // any logged-in user - maybe UI preferences, feature flags

        [JsonPropertyName("Administrator")]
        Administrator    // admin panel only - WAF config, security policies
    }
}
