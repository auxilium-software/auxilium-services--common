using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SystemSettingVisibilityEnum
    {
        [JsonPropertyName("public")]
        Public,          // unauthenticated - logos, contact info, branding

        // [JsonPropertyName("authenticated")]
        // Authenticated,   // any logged-in user - maybe UI preferences, feature flags

        [JsonPropertyName("administrator")]
        Administrator    // admin panel only - WAF config, security policies
    }
}
