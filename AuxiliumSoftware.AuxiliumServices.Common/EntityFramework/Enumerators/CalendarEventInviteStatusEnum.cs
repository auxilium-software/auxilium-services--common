using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CalendarEventInviteStatusEnum
    {
        [JsonPropertyName("pending")]
        Pending,

        [JsonPropertyName("accepted")]
        Accepted,

        [JsonPropertyName("declined")]
        Declined,

        [JsonPropertyName("tenative")]
        Tentative,
    }
}
