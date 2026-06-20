using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.Common.DataTransferObjects
{
    public class AdditionalPropertySubStructureDTO
    {
        [JsonPropertyName("id")]
        public required Guid Id { get; set; }

        [JsonPropertyName("createdAt")]
        public required DateTime CreatedAt { get; set; }

        [JsonPropertyName("createdBy")]
        public Guid? CreatedBy { get; set; }

        [JsonPropertyName("updatedAt")]
        public DateTime? UpdatedAt { get; set; }

        [JsonPropertyName("lastUpdatedBy")]
        public Guid? LastUpdatedBy { get; set; }





        [JsonPropertyName("displayName")]
        public required string DisplayName { get; set; }

        [JsonPropertyName("content")]
        public required string Content { get; set; }

        [JsonPropertyName("contentType")]
        public required string ContentType { get; set; }





        [JsonPropertyName("dataEnumeratorId")]
        public Guid? DataEnumeratorId { get; set; }

        [JsonPropertyName("displayValue")]
        public string? DisplayValue { get; set; }

        [JsonPropertyName("dataEnumeratorDisplayName")]
        public string? DataEnumeratorDisplayName { get; set; }
    }
}
