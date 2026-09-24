using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum UserEntityTypeEnum
    {
        [JsonPropertyName("User")]
        User,

        [JsonPropertyName("User.AdditionalProperty")]
        User_AdditionalProperty,

        [JsonPropertyName("User.File")]
        User_File,
    }
}
