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

        public static string NormaliseLanguageCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Language code must not be empty", nameof(code));

            var parts = code.Trim().Split('-');
            parts[0] = parts[0].ToLowerInvariant(); // language subtag: cy, en
            if (parts.Length > 1)
                parts[1] = parts[1].ToUpperInvariant(); // region subtag: GB
            return string.Join('-', parts); // cy-gb -> cy-GB
        }
    }
}
