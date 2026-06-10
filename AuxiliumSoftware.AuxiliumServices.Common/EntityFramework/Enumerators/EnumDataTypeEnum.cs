using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum EnumDataTypeEnum
    {
        [JsonPropertyName("string")]
        String,

        [JsonPropertyName("integer")]
        Integer,

        [JsonPropertyName("decimal")]
        Decimal,

        [JsonPropertyName("boolean")]
        Boolean,
    }
}
