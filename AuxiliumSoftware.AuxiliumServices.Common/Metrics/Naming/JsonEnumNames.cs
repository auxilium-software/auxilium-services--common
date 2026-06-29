using System.Reflection;
using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.Common.Metrics.Naming
{
    public static class JsonEnumNames<TEnum> where TEnum : struct, Enum
    {
        private static readonly Dictionary<TEnum, string> _toName = new();
        private static readonly Dictionary<string, TEnum> _fromName = new(StringComparer.OrdinalIgnoreCase);

        static JsonEnumNames()
        {
            foreach (var field in typeof(TEnum).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                var value = (TEnum)field.GetValue(null)!;
                var name = field.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? field.Name;
                _toName[value] = name;
                _fromName[name] = value;
            }
        }

        public static string Name(TEnum value) => _toName[value];

        public static bool TryParse(string name, out TEnum value) => _fromName.TryGetValue(name, out value);
    }
}
