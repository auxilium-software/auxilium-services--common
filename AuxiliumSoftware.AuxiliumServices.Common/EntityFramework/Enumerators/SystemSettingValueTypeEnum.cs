using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SystemSettingValueTypeEnum
    {
        [JsonPropertyName("String")]
        String,

        [JsonPropertyName("Int")]
        Int,

        [JsonPropertyName("Bool")]
        Bool,

        [JsonPropertyName("Decimal")]
        Decimal,

        [JsonPropertyName("Day")]
        Day,
        [JsonPropertyName("Date")]
        Date,
        [JsonPropertyName("Time")]
        Time,
        [JsonPropertyName("Datetime")]
        Datetime,
        [JsonPropertyName("DayArray")]
        DayArray,

        /*
        [JsonPropertyName("stringArray")]
        StringArray,

        [JsonPropertyName("json")]
        Json
        */
    }
}
