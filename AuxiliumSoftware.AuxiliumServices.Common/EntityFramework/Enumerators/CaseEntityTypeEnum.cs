using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CaseEntityTypeEnum
    {
        [JsonPropertyName("/case")]
        Case,

        [JsonPropertyName("/case/additional_property")]
        Case_AdditionalProperty,

        [JsonPropertyName("/case/worker")]
        Case_Worker,

        [JsonPropertyName("/case/client")]
        Case_Client,

        [JsonPropertyName("/case/message")]
        Case_Message,

        [JsonPropertyName("/case/file")]
        Case_File,

        [JsonPropertyName("/case/timeline_entry")]
        Case_TimelineEntry,

        [JsonPropertyName("/case/todo")]
        Case_Todo,
    }
}
