using AuxiliumSoftware.AuxiliumServices.Common.Attributes;
using AuxiliumSoftware.AuxiliumServices.Common.DataTransferObjects;
using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework;
using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels;
using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace AuxiliumSoftware.AuxiliumServices.Common.Services.Implementations
{
    public class SystemSettingsService : ISystemSettingsService
    {
        private readonly AuxiliumDbContext _db;

        /// <summary>
        /// Static metadata cache built from SystemSettingKeyEnum reflection.
        /// Keyed by JSON key for easy lookup when serving API requests.
        /// Does NOT store runtime DB values, so it never goes stale.
        /// </summary>
        private static readonly Lazy<Dictionary<string, SettingMetadata>> _metadataByJsonKey = new(() => BuildMetadataCache());

        public SystemSettingsService(AuxiliumDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<SystemSettingDTO>> GetVisibleSettingsAsync(
            SystemSettingVisibilityEnum callerVisibility,
            CancellationToken ct = default)
        {
            var visibleKeys = _metadataByJsonKey.Value
                .Where(kvp => kvp.Value.Visibility <= callerVisibility)
                .ToList();

            // batch-fetch all DB overrides in one query rather than N+1
            var enumKeys = visibleKeys
                .Select(kvp => kvp.Value.EnumValue)
                .ToList();

            var dbOverrides = await _db.System_Settings
                .AsNoTracking()
                .Where(s => enumKeys.Contains(s.ConfigKey))
                .GroupBy(s => s.ConfigKey)
                .Select(g => g.OrderByDescending(s => s.CreatedAtUtc).First())
                .ToDictionaryAsync(s => s.ConfigKey, s => s, ct);

            return visibleKeys.Select(kvp =>
            {
                var meta = kvp.Value;
                object? value;

                if (dbOverrides.TryGetValue(meta.EnumValue, out var dbSetting))
                {
                    value = DeserializeValue(dbSetting.ConfigValue, meta.ValueType);
                }
                else
                {
                    value = ResolveDefaultValue(meta);
                }

                return BuildDto(kvp.Key, meta, value, callerVisibility);
            });
        }

        public async Task<SystemSettingDTO?> GetVisibleSettingByKeyAsync(
            string jsonKey,
            SystemSettingVisibilityEnum callerVisibility,
            CancellationToken ct = default)
        {
            if (!_metadataByJsonKey.Value.TryGetValue(jsonKey, out var meta))
                return null;

            // return null rather than 403 to avoid confirming key existence
            if (meta.Visibility > callerVisibility)
                return null;

            var value = await GetValueAsync(meta.EnumValue, ct);

            return BuildDto(jsonKey, meta, value, callerVisibility);
        }

        public async Task<dynamic> GetValueAsync(SystemSettingKeyEnum key, CancellationToken ct = default)
        {
            var typeAttr = GetAttribute<SystemSettingExpectedValueTypeAttribute>(key) ?? throw new InvalidOperationException($"No expected value type specified for key '{key}'");

            var setting = await _db.System_Settings
                .AsNoTracking()
                .Where(s => s.ConfigKey == key)
                .OrderByDescending(s => s.CreatedAtUtc)
                .FirstOrDefaultAsync(ct);

            if (setting is not null)
            {
                return DeserializeValue(setting.ConfigValue, typeAttr.ValueType);
            }

            var defaultAttr = GetAttribute<SystemSettingDefaultValueAttribute>(key);
            if (defaultAttr is null)
                throw new InvalidOperationException(
                    $"No value found in database for key '{key}' and no default attribute specified");

            if (typeAttr.ValueType == SystemSettingValueTypeEnum.StringArray)
            {
                var stringValue = defaultAttr.DefaultValue?.ToString() ?? "";
                return string.IsNullOrEmpty(stringValue)
                    ? new List<string>()
                    : stringValue.Split(',').Select(s => s.Trim()).ToList();
            }

            return defaultAttr.DefaultValue!;
        }

        public async Task<string> GetStringAsync(SystemSettingKeyEnum key, CancellationToken ct = default)
        {
            var value = await GetValueAsync(key, ct);
            return (string)value;
        }

        public async Task<int> GetIntAsync(SystemSettingKeyEnum key, CancellationToken ct = default)
        {
            var value = await GetValueAsync(key, ct);
            return Convert.ToInt32(value);
        }

        public async Task<bool> GetBoolAsync(SystemSettingKeyEnum key, CancellationToken ct = default)
        {
            var value = await GetValueAsync(key, ct);
            return (bool)value;
        }

        internal async Task<T?> GetAsync<T>(SystemSettingKeyEnum key, CancellationToken ct = default)
        {
            var value = await GetValueAsync(key, ct);

            if (value is T typedValue)
                return typedValue;

            if (value is JsonElement jsonElement)
                return jsonElement.Deserialize<T>();

            return (T)Convert.ChangeType(value, typeof(T));
        }

        public async Task SetAsync<T>(
            SystemSettingKeyEnum key,
            T value,
            Guid? modifiedBy,
            string reasonForModification,
            CancellationToken ct = default)
        {
            var jsonValue = JsonSerializer.Serialize(value);
            var valueType = InferValueType(value);

            _db.System_Settings.Add(new SystemSettingEntityModel
            {
                Id = Guid.NewGuid(),
                ConfigKey = key,
                ConfigValue = jsonValue,
                ValueType = valueType,
                CreatedAtUtc = DateTime.UtcNow,
                CreatedBy = modifiedBy,
                ReasonForModification = reasonForModification
            });

            await _db.SaveChangesAsync(ct);
        }

        private static SystemSettingDTO BuildDto(
            string jsonKey,
            SettingMetadata meta,
            object? value,
            SystemSettingVisibilityEnum callerVisibility)
        {
            var dto = new SystemSettingDTO
            {
                Key = jsonKey,
                Value = value,
                ValueType = meta.ValueType
            };

            // only expose description/recommendation to admins - public/authenticated callers just need the values
            if (callerVisibility == SystemSettingVisibilityEnum.Administrator)
            {
                dto.Description = meta.Description;
                dto.Recommendation = meta.Recommendation;
            }

            return dto;
        }

        private static object? ResolveDefaultValue(SettingMetadata meta)
        {
            if (meta.DefaultValue is null)
                return null;

            if (meta.ValueType == SystemSettingValueTypeEnum.StringArray)
            {
                var stringValue = meta.DefaultValue.ToString() ?? "";
                return string.IsNullOrEmpty(stringValue)
                    ? new List<string>()
                    : stringValue.Split(',').Select(s => s.Trim()).ToList();
            }

            return meta.DefaultValue;
        }

        private static dynamic DeserializeValue(string json, SystemSettingValueTypeEnum valueType)
        {
            return valueType switch
            {
                SystemSettingValueTypeEnum.String => JsonSerializer.Deserialize<string>(json)!,
                SystemSettingValueTypeEnum.Int => JsonSerializer.Deserialize<int>(json),
                SystemSettingValueTypeEnum.Bool => JsonSerializer.Deserialize<bool>(json),
                SystemSettingValueTypeEnum.Decimal => JsonSerializer.Deserialize<decimal>(json),
                SystemSettingValueTypeEnum.StringArray => JsonSerializer.Deserialize<List<string>>(json)!,
                SystemSettingValueTypeEnum.Json => JsonSerializer.Deserialize<JsonElement>(json),
                _ => throw new ArgumentOutOfRangeException(
                    nameof(valueType), $"Unsupported value type: {valueType}")
            };
        }

        private static T? GetAttribute<T>(SystemSettingKeyEnum key) where T : Attribute
        {
            var memberInfo = typeof(SystemSettingKeyEnum).GetField(key.ToString());
            return memberInfo?.GetCustomAttribute<T>();
        }

        private static SystemSettingValueTypeEnum InferValueType<T>(T value)
        {
            return value switch
            {
                string => SystemSettingValueTypeEnum.String,
                int or long => SystemSettingValueTypeEnum.Int,
                bool => SystemSettingValueTypeEnum.Bool,
                decimal or float or double => SystemSettingValueTypeEnum.Decimal,
                IEnumerable<string> => SystemSettingValueTypeEnum.StringArray,
                _ => SystemSettingValueTypeEnum.Json
            };
        }

        private static Dictionary<string, SettingMetadata> BuildMetadataCache()
        {
            var result = new Dictionary<string, SettingMetadata>();

            foreach (var enumValue in Enum.GetValues<SystemSettingKeyEnum>())
            {
                var field = typeof(SystemSettingKeyEnum).GetField(enumValue.ToString())!;

                var jsonKey = field.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name
                    ?? enumValue.ToString();

                result[jsonKey] = new SettingMetadata
                {
                    EnumValue = enumValue,
                    Visibility = field.GetCustomAttribute<SystemSettingVisibilityAttribute>()?.Visibility
                        ?? SystemSettingVisibilityEnum.Administrator,
                    ValueType = field.GetCustomAttribute<SystemSettingExpectedValueTypeAttribute>()?.ValueType
                        ?? SystemSettingValueTypeEnum.String,
                    DefaultValue = field.GetCustomAttribute<SystemSettingDefaultValueAttribute>()?.DefaultValue,
                    Description = field.GetCustomAttribute<SystemSettingDescriptionAttribute>()?.Description,
                    Recommendation = field.GetCustomAttribute<SystemSettingRecommendationAttribute>()?.Recommendation
                };
            }

            return result;
        }
    }

    internal class SettingMetadata
    {
        public SystemSettingKeyEnum EnumValue { get; set; }
        public SystemSettingVisibilityEnum Visibility { get; set; }
        public SystemSettingValueTypeEnum ValueType { get; set; }
        public object? DefaultValue { get; set; }
        public string? Description { get; set; }
        public string? Recommendation { get; set; }
    }
}
