using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.Common.Utilities
{
    public static class BuiltInAdditionalPropertyDefinitionCatalogue
    {
        public const string DefinitionsResourceName = "AuxiliumSoftware.AuxiliumServices.Common.Resources.AdditionalPropertyDefinitions.definitions.json";
        public const string LanguageResourcePrefix = "AuxiliumSoftware.AuxiliumServices.Common.Resources.AdditionalPropertyDefinitions.Languages.";
        private const string LanguageResourceSuffix = ".json";

        #region ========================= PUBLIC SHAPE =========================
        public class Entry
        {
            public required string CanonicalName { get; init; }
            public required AdditionalPropertyDataTypeEnum DataType { get; init; }
            public string? Description { get; init; }

            public required IReadOnlyDictionary<string, string> Translations { get; init; }

            public required IReadOnlyList<string> Aliases { get; init; }
        }

        public static IReadOnlyList<Entry> User => _catalogue.Value.User;
        public static IReadOnlyList<Entry> Case => _catalogue.Value.Case;

        public static IReadOnlyList<string> LanguageCodes => _catalogue.Value.LanguageCodes;
        #endregion
        #region ========================= FILE SHAPES =========================
        private class DefinitionsFile
        {
            [JsonPropertyName("user")]
            public List<DefinitionFileEntry> User { get; set; } = new();

            [JsonPropertyName("case")]
            public List<DefinitionFileEntry> Case { get; set; } = new();
        }

        private class DefinitionFileEntry
        {
            [JsonPropertyName("canonicalName")]
            public required string CanonicalName { get; set; }

            [JsonPropertyName("dataType")]
            public required AdditionalPropertyDataTypeEnum DataType { get; set; }

            [JsonPropertyName("description")]
            public string? Description { get; set; }
        }

        private class LanguageFile
        {
            [JsonPropertyName("user")]
            public Dictionary<string, LanguageFileEntry> User { get; set; } = new();

            [JsonPropertyName("case")]
            public Dictionary<string, LanguageFileEntry> Case { get; set; } = new();
        }

        private class LanguageFileEntry
        {
            [JsonPropertyName("label")]
            public required string Label { get; set; }

            [JsonPropertyName("aliases")]
            public List<string> Aliases { get; set; } = new();
        }
        #endregion
        #region ========================= LOADING =========================
        private record LoadedCatalogue(IReadOnlyList<Entry> User, IReadOnlyList<Entry> Case, IReadOnlyList<string> LanguageCodes);

        private static readonly Lazy<LoadedCatalogue> _catalogue = new(Load);

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            Converters = { new JsonStringEnumConverter() },
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true,
        };

        private static LoadedCatalogue Load()
        {
            var assembly = typeof(BuiltInAdditionalPropertyDefinitionCatalogue).Assembly;

            var definitions = Read<DefinitionsFile>(assembly, DefinitionsResourceName);

            var languages = assembly.GetManifestResourceNames()
                .Where(n => n.StartsWith(LanguageResourcePrefix, StringComparison.Ordinal) && n.EndsWith(LanguageResourceSuffix, StringComparison.Ordinal))
                .OrderBy(n => n, StringComparer.Ordinal)
                .Select(n => (LanguageCode: n[LanguageResourcePrefix.Length..^LanguageResourceSuffix.Length], File: Read<LanguageFile>(assembly, n)))
                .ToList();

            return new LoadedCatalogue(
                Merge("user", definitions.User, languages.Select(l => (l.LanguageCode, l.File.User))),
                Merge("case", definitions.Case, languages.Select(l => (l.LanguageCode, l.File.Case))),
                languages.Select(l => l.LanguageCode).ToList()
            );
        }

        private static List<Entry> Merge(
            string subject,
            List<DefinitionFileEntry> definitions,
            IEnumerable<(string LanguageCode, Dictionary<string, LanguageFileEntry> Entries)> languages
        )
        {
            var duplicate = definitions.GroupBy(d => d.CanonicalName).FirstOrDefault(g => g.Count() > 1);
            if (duplicate != null)
                throw new InvalidOperationException($"Built-in {subject} definition '{duplicate.Key}' is defined more than once");

            var known = definitions.Select(d => d.CanonicalName).ToHashSet();
            var translations = definitions.ToDictionary(d => d.CanonicalName, _ => new Dictionary<string, string>());
            var aliases = definitions.ToDictionary(d => d.CanonicalName, _ => new List<string>());

            foreach (var (languageCode, entries) in languages)
            {
                foreach (var (canonicalName, entry) in entries)
                {
                    if (!known.Contains(canonicalName))
                        throw new InvalidOperationException($"Language file {languageCode} has a {subject} entry for '{canonicalName}', which isn't a built-in definition");

                    translations[canonicalName][languageCode] = entry.Label;
                    aliases[canonicalName].Add(entry.Label);
                    aliases[canonicalName].AddRange(entry.Aliases);
                }
            }

            return definitions
                .Select(d => new Entry
                {
                    CanonicalName = d.CanonicalName,
                    DataType = d.DataType,
                    Description = d.Description,
                    Translations = translations[d.CanonicalName],
                    Aliases = aliases[d.CanonicalName],
                })
                .ToList();
        }

        private static T Read<T>(System.Reflection.Assembly assembly, string resourceName)
        {
            using var stream = assembly.GetManifestResourceStream(resourceName)
                ?? throw new InvalidOperationException($"Embedded resource {resourceName} not found");

            return JsonSerializer.Deserialize<T>(stream, _jsonOptions)
                ?? throw new InvalidOperationException($"Embedded resource {resourceName} is empty");
        }
        #endregion
    }
}

