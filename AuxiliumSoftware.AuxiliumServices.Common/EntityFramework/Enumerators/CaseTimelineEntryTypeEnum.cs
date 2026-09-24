using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CaseTimelineEntryTypeEnum
    {
        [JsonPropertyName("Note.User")]
        Note_User,

        [JsonPropertyName("Note.System")]
        Note_System,
    }
}
