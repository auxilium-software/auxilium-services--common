using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CaseEntityTypeEnum
    {
        [JsonPropertyName("Case.Itself")]
        Case,

        [JsonPropertyName("Case.AdditionalProperty")]
        Case_AdditionalProperty,

        [JsonPropertyName("Case.Worker")]
        Case_Worker,

        [JsonPropertyName("Case.Client")]
        Case_Client,

        [JsonPropertyName("Case.Message")]
        Case_Message,

        [JsonPropertyName("Case.File")]
        Case_File,

        [JsonPropertyName("Case.TimelineEntry")]
        Case_TimelineEntry,

        [JsonPropertyName("Case.Todo")]
        Case_Todo,
    }
}
