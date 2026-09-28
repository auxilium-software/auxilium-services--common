using AuxiliumSoftware.AuxiliumServices.Common.Attributes;
using AuxiliumSoftware.AuxiliumServices.Common.DataTransferObjects;
using AuxiliumSoftware.AuxiliumServices.Common.Enumerators;
using AuxiliumSoftware.AuxiliumServices.Common.Exceptions;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.Common.Utilities
{
    public sealed class DataMergeFieldResolver<TEntity> where TEntity : class
    {
        public sealed record Field(string Key, PropertyInfo Property, DataMergeResolvableAttribute Attribute);

        public sealed record Outcome(
            List<object> Resolutions,
            List<(string PropertyName, string? Previous, string? Next)> Changes,
            IReadOnlyDictionary<string, DataMergeSourceEnum> Sources
        );

        private const string CombineSeparator = "\n\n";

        public IReadOnlyList<Field> Fields { get; }





        public DataMergeFieldResolver()
        {
            List<Field> fields = typeof(TEntity)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(p => (Property: p, Attribute: p.GetCustomAttribute<DataMergeResolvableAttribute>()))
                .Where(x => x.Attribute is not null)
                .Select(x => new Field(
                    char.ToLowerInvariant(x.Property.Name[0]) + x.Property.Name[1..],
                    x.Property,
                    x.Attribute!
                ))
                .ToList();

            foreach (Field field in fields.Where(f => f.Attribute.AllowCombine && f.Property.PropertyType != typeof(string)))
            {
                throw new InvalidOperationException(
                    $"{typeof(TEntity).Name}.{field.Property.Name} allows combining on merge, but only string properties can be combined."
                );
            }

            Fields = fields;
        }





        #region ========================= PREVIEW =========================
        public List<DataMergeFieldConflictDTO> BuildPreviewFields(TEntity survivor, TEntity duplicate)
        {
            return Fields.Select(field =>
            {
                object? survivorValue = field.Property.GetValue(survivor);
                object? duplicateValue = field.Property.GetValue(duplicate);

                return new DataMergeFieldConflictDTO
                {
                    Field = field.Key,
                    SurvivorValue = FormatValue(survivorValue),
                    DuplicateValue = FormatValue(duplicateValue),
                    Differs = !Equals(survivorValue, duplicateValue),
                    CanCombine = field.Attribute.AllowCombine,
                    SuggestedSource = SuggestSource(field, survivorValue, duplicateValue),
                };
            }).ToList();
        }
        #endregion





        #region ========================= CHOICES =========================
        public Dictionary<string, DataMergeSourceEnum> ParseChoices(IReadOnlyDictionary<string, DataMergeSourceEnum>? fieldChoices)
        {
            Dictionary<string, DataMergeSourceEnum> result = new(StringComparer.OrdinalIgnoreCase);
            if (fieldChoices is null)
            {
                return result;
            }

            foreach ((string key, DataMergeSourceEnum source) in fieldChoices)
            {
                Field field = Fields.FirstOrDefault(f => string.Equals(f.Key, key, StringComparison.OrdinalIgnoreCase))
                    ?? throw new DataMergeRejectedException(
                        DataMergeRejectionReasonEnum.Invalid,
                        $"\"{key}\" is not a field that can be chosen during a merge."
                    );

                if (!Enum.IsDefined(source))
                {
                    throw new DataMergeRejectedException(
                        DataMergeRejectionReasonEnum.Invalid,
                        $"The choice for \"{field.Key}\" must be one of: {string.Join(", ", Enum.GetNames<DataMergeSourceEnum>())}."
                    );
                }

                if (source == DataMergeSourceEnum.Combined && !field.Attribute.AllowCombine)
                {
                    throw new DataMergeRejectedException(
                        DataMergeRejectionReasonEnum.Invalid,
                        $"\"{field.Key}\" cannot be combined; choose one side."
                    );
                }

                result[field.Key] = source;
            }

            return result;
        }

        public Outcome Apply(TEntity survivor, TEntity duplicate, IReadOnlyDictionary<string, DataMergeSourceEnum> choices)
        {
            List<object> resolutions = new();
            List<(string PropertyName, string? Previous, string? Next)> changes = new();
            Dictionary<string, DataMergeSourceEnum> sources = new(StringComparer.OrdinalIgnoreCase);

            foreach (Field field in Fields)
            {
                object? survivorValue = field.Property.GetValue(survivor);
                object? duplicateValue = field.Property.GetValue(duplicate);

                DataMergeSourceEnum source = choices.TryGetValue(field.Key, out DataMergeSourceEnum chosen)
                    ? chosen
                    : SuggestSource(field, survivorValue, duplicateValue);

                object? resolved = source switch
                {
                    DataMergeSourceEnum.Survivor => survivorValue,
                    DataMergeSourceEnum.Duplicate => duplicateValue,
                    DataMergeSourceEnum.Combined => Combine((string?)survivorValue, (string?)duplicateValue),
                    _ => throw new InvalidOperationException($"Unhandled merge source {source}."),
                };

                if (!Equals(survivorValue, resolved))
                {
                    field.Property.SetValue(survivor, resolved);
                    changes.Add((field.Property.Name, FormatValue(survivorValue), FormatValue(resolved)));
                }

                sources[field.Key] = source;
                resolutions.Add(new { field = field.Key, source, value = FormatValue(resolved) });
            }

            return new Outcome(resolutions, changes, sources);
        }

        public static DataMergeSourceEnum SuggestSource(Field field, object? survivorValue, object? duplicateValue)
        {
            return field.Attribute.Default switch
            {
                DataMergeFieldDefaultEnum.PreferHigherValue =>
                    Comparer<object?>.Default.Compare(duplicateValue, survivorValue) > 0
                        ? DataMergeSourceEnum.Duplicate
                        : DataMergeSourceEnum.Survivor,

                _ => IsEmpty(survivorValue) && !IsEmpty(duplicateValue)
                        ? DataMergeSourceEnum.Duplicate
                        : DataMergeSourceEnum.Survivor,
            };
        }

        public static string? Combine(string? survivorValue, string? duplicateValue)
        {
            if (string.IsNullOrWhiteSpace(duplicateValue) || string.Equals(survivorValue, duplicateValue, StringComparison.Ordinal))
            {
                return survivorValue;
            }
            if (string.IsNullOrWhiteSpace(survivorValue))
            {
                return duplicateValue;
            }

            return survivorValue.TrimEnd() + CombineSeparator + duplicateValue.TrimStart();
        }
        #endregion





        #region ========================= FINGERPRINT & SNAPSHOT =========================
        public string ComputeFingerprint(TEntity survivor, TEntity duplicate, Func<TEntity, IEnumerable<string?>> identity)
        {
            StringBuilder builder = new();

            void Append(string? value) =>
                builder.Append(value?.Length ?? -1).Append(':').Append(value).Append(';');

            foreach (TEntity entity in new[] { survivor, duplicate })
            {
                foreach (string? part in identity(entity))
                {
                    Append(part);
                }

                foreach (Field field in Fields)
                {
                    Append(FormatValue(field.Property.GetValue(entity)));
                }
            }

            return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(builder.ToString())));
        }

        public static Dictionary<string, string?> SnapshotScalars(IModel model, TEntity entity, params string[] excludedProperties)
        {
            return model.FindEntityType(typeof(TEntity))!
                .GetProperties()
                .Where(p => p.PropertyInfo is not null && !excludedProperties.Contains(p.Name))
                .ToDictionary(
                    p => p.Name,
                    p => FormatValue(p.PropertyInfo!.GetValue(entity))
                );
        }
        #endregion





        #region ========================= FORMATTING =========================
        public static bool IsEmpty(object? value) =>
            value is null
            || (
                value is string s
                && string.IsNullOrWhiteSpace(s)
            );

        public static string? FormatValue(object? value) => value switch
        {
            null => null,
            Enum enumValue => enumValue.GetType().GetField(enumValue.ToString())?.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? enumValue.ToString(),
            bool boolean => boolean ? "true" : "false",
            DateOnly date => date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            DateTime dateTime => dateTime.ToString("O", CultureInfo.InvariantCulture),
            IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
            _ => value.ToString(),
        };
        #endregion
    }
}
