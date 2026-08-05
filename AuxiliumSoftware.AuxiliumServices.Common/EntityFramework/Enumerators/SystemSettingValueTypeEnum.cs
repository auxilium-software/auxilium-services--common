using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SystemSettingValueTypeEnum
    {
        [JsonPropertyName("string")]
        String,

        [JsonPropertyName("int")]
        Int,

        [JsonPropertyName("bool")]
        Bool,

        [JsonPropertyName("decimal")]
        Decimal,

        [JsonPropertyName("day")]
        Day,
        [JsonPropertyName("date")]
        Date,
        [JsonPropertyName("time")]
        Time,
        [JsonPropertyName("datetime")]
        Datetime,
        [JsonPropertyName("day_array")]
        DayArray,

        /*
        [JsonPropertyName("stringArray")]
        StringArray,

        [JsonPropertyName("json")]
        Json
        */
    }
}
