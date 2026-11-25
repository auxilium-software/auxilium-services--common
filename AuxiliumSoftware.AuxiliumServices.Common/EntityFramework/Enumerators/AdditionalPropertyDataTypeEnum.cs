using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AdditionalPropertyDataTypeEnum
    {
        [JsonPropertyName("Text")]
        Text,

        [JsonPropertyName("LongText")]
        LongText,

        [JsonPropertyName("Date")]
        Date,

        [JsonPropertyName("Number")]
        Number,

        [JsonPropertyName("Boolean")]
        Boolean,

        [JsonPropertyName("EmailAddress")]
        EmailAddress,

        [JsonPropertyName("TelephoneNumber")]
        TelephoneNumber,

        [JsonPropertyName("PostalAddress")]
        PostalAddress,

        [JsonPropertyName("DataEnumeratorValue")]
        DataEnumeratorValue,
    }
}

