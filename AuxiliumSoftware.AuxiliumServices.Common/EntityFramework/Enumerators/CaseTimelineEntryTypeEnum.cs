using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CaseTimelineEntryTypeEnum
    {
        [JsonPropertyName("note.user")]
        Note_User,

        [JsonPropertyName("note.system")]
        Note_System,
    }
}
