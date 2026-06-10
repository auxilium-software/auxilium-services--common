using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators;
using System.Buffers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;


namespace AuxiliumSoftware.AuxiliumServices.Common.Utilities
{
    public static class EnumValueUtilities
    {
        public static string Canonicalise(string json)
        {
            using var doc = JsonDocument.Parse(json);
            var buffer = new ArrayBufferWriter<byte>();
            using (var writer = new Utf8JsonWriter(buffer))
                Write(doc.RootElement, writer);
            return Encoding.UTF8.GetString(buffer.WrittenSpan);
        }

        private static void Write(JsonElement el, Utf8JsonWriter w)
        {
            switch (el.ValueKind)
            {
                case JsonValueKind.Object:
                    w.WriteStartObject();
                    foreach (var p in el.EnumerateObject().OrderBy(p => p.Name, StringComparer.Ordinal))
                    {
                        w.WritePropertyName(p.Name);
                        Write(p.Value, w);
                    }
                    w.WriteEndObject();
                    break;
                case JsonValueKind.Array:
                    w.WriteStartArray();
                    foreach (var item in el.EnumerateArray()) Write(item, w);
                    w.WriteEndArray();
                    break;
                default:
                    el.WriteTo(w);
                    break;
            }
        }

        public static string Hash(string canonicalJson) =>
            Convert.ToHexString(SHA512.HashData(Encoding.UTF8.GetBytes(canonicalJson))).ToLowerInvariant();

        public static void ValidateAgainstType(string canonicalJson, EnumDataTypeEnum dataType)
        {
            using var doc = JsonDocument.Parse(canonicalJson);
            var kind = doc.RootElement.ValueKind;

            var ok = dataType switch
            {
                EnumDataTypeEnum.String => kind == JsonValueKind.String,
                EnumDataTypeEnum.Integer => kind == JsonValueKind.Number
                                            && doc.RootElement.TryGetInt64(out _),
                EnumDataTypeEnum.Decimal => kind == JsonValueKind.Number,
                EnumDataTypeEnum.Boolean => kind is JsonValueKind.True or JsonValueKind.False,
                _ => throw new ArgumentOutOfRangeException(nameof(dataType), dataType, "Unhandled enumerator data type")
            };

            if (!ok)
                throw new InvalidOperationException(
                    $"Value '{canonicalJson}' is not valid for enumerator data type {dataType} (got JSON kind {kind})");
        }
    }
}
