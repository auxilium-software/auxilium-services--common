using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SystemBulletinMessageSeverityEnum
    {
        [JsonPropertyName("Informational")]
        Informational,

        [JsonPropertyName("Warning")]
        Warning,

        [JsonPropertyName("Critical")]
        Critical,
    }
}
