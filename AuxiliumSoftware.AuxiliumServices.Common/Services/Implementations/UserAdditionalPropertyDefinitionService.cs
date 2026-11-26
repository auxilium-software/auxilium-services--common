using AuxiliumSoftware.AuxiliumServices.Common.DataTransferObjects;
using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework;
using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels;
using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators;
using AuxiliumSoftware.AuxiliumServices.Common.Enumerators;
using AuxiliumSoftware.AuxiliumServices.Common.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AuxiliumSoftware.AuxiliumServices.Common.Services.Implementations
{
    public class UserAdditionalPropertyDefinitionService : IUserAdditionalPropertyDefinitionService
    {
        private const int LinkBatchSize = 500;

        private readonly AuxiliumDbContext _db;
        private readonly ILogger<UserAdditionalPropertyDefinitionService> _logger;

        public UserAdditionalPropertyDefinitionService(
            AuxiliumDbContext db,
            ILogger<UserAdditionalPropertyDefinitionService> logger
        )
        {
            this._db = db;
            this._logger = logger;
        }

        #region ========================= READ OPERATIONS =========================
        public async Task<List<UserAdditionalPropertyDefinitionEntityModel>> GetDefinitionsAsync(bool includeInactive = true, CancellationToken ct = default)
        {
            try
            {
                return await _db.WithinTenancy_UserAdditionalProperty_Definitions
                    .Include(d => d.Translations)
                    .Include(d => d.Aliases)
                    .Where(d => includeInactive || d.IsActive)
                    .OrderBy(d => d.SortOrder)
                    .ThenBy(d => d.CanonicalName)
                    .ToListAsync(ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get user additional property definitions");
                throw;
            }
        }

        public async Task<UserAdditionalPropertyDefinitionEntityModel?> GetDefinitionAsync(Guid definitionId, CancellationToken ct = default)
        {
            try
            {
                return await _db.WithinTenancy_UserAdditionalProperty_Definitions
                    .Include(d => d.Translations)
                    .Include(d => d.Aliases)
                    .FirstOrDefaultAsync(d => d.Id == definitionId, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get user additional property definition {DefinitionId}", definitionId);
                throw;
            }
        }

        public async Task<List<UnlinkedAdditionalPropertyNameDTO>> GetUnlinkedPropertyNamesAsync(CancellationToken ct = default)
        {
            try
            {
                List<string> names = await _db.WithinTenancy_UserAdditionalProperties
                    .Where(p => p.DefinitionId == null)
                    .Select(p => p.DisplayName)
                    .ToListAsync(ct);

                return names
                    .Select(n => (DisplayName: n, Key: PropertyNameHandlingUtilities.NormaliseForMatching(n)))
                    .Where(n => n.Key.Length > 0)
                    .GroupBy(n => n.Key)
                    .Select(g => new UnlinkedAdditionalPropertyNameDTO
                    {
                        NormalisedName = g.Key,
                        DisplayNames = g
                            .GroupBy(n => n.DisplayName)
                            .OrderByDescending(v => v.Count())
                            .ThenBy(v => v.Key, StringComparer.Ordinal)
                            .Select(v => v.Key)
                            .ToList(),
                        PropertyCount = g.Count(),
                    })
                    .OrderByDescending(g => g.PropertyCount)
                    .ThenBy(g => g.NormalisedName, StringComparer.Ordinal)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get unlinked user additional property names");
                throw;
            }
        }
        #endregion
        #region ========================= DEFINITION OPERATIONS =========================
        public async Task<UserAdditionalPropertyDefinitionEntityModel> CreateDefinitionAsync(
            Guid actorUserId,
            string canonicalName,
            AdditionalPropertyDataTypeEnum dataType,
            string? description = null,
            Guid? dataEnumeratorId = null,
            Guid? defaultGroupId = null,
            IReadOnlyDictionary<string, string>? translations = null,
            IReadOnlyList<string>? aliases = null,
            CancellationToken ct = default
        )
        {
            try
            {
                canonicalName = ValidateCanonicalName(canonicalName);
                await ValidateReferencesAsync(dataType, dataEnumeratorId, defaultGroupId, ct);

                if (await _db.WithinTenancy_UserAdditionalProperty_Definitions.AnyAsync(d => d.CanonicalName == canonicalName, ct))
                {
                    throw new InvalidOperationException($"A user additional property definition named '{canonicalName}' already exists");
                }

                Dictionary<string, string> normalisedAliases = NormaliseAliases(aliases);
                await EnsureAliasesAvailableAsync(normalisedAliases.Keys, null, ct);

                int nextSortOrder = (await _db.WithinTenancy_UserAdditionalProperty_Definitions
                    .MaxAsync(d => (int?)d.SortOrder, ct) ?? -1) + 1;

                DateTime now = DateTime.UtcNow;

                UserAdditionalPropertyDefinitionEntityModel definition = new()
                {
                    Id = UUIDUtilities.GenerateV5(DatabaseObjectTypeEnum.WithinTenancy_User_AdditionalProperty_Definition),
                    CanonicalName = canonicalName,
                    Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
                    DataType = dataType,
                    DataEnumeratorId = dataEnumeratorId,
                    DefaultGroupId = defaultGroupId,
                    IsBuiltIn = false,
                    IsActive = true,
                    SortOrder = nextSortOrder,
                    CreatedByUserId = actorUserId,
                    CreatedAtUtc = now,
                    Translations = new List<UserAdditionalPropertyDefinitionTranslationEntityModel>(),
                    Aliases = new List<UserAdditionalPropertyDefinitionAliasEntityModel>(),
                };

                AddTranslations(definition, translations, actorUserId, now);
                foreach (var (normalisedAlias, alias) in normalisedAliases)
                {
                    definition.Aliases.Add(NewAlias(definition.Id, alias, normalisedAlias, actorUserId, now));
                }

                _db.WithinTenancy_UserAdditionalProperty_Definitions.Add(definition);

                int linked = await LinkUnlinkedPropertiesAsync(definition.Id, normalisedAliases.Keys.ToHashSet(), actorUserId, now, ct);

                await _db.SaveChangesAsync(ct);

                _logger.LogInformation(
                    "Created user additional property definition {DefinitionId} ({CanonicalName}), linking {Count} properties",
                    definition.Id, canonicalName, linked
                );

                return definition;
            }
            catch (Exception ex) when (ex is not ArgumentException and not InvalidOperationException and not KeyNotFoundException)
            {
                _logger.LogError(ex, "Failed to create user additional property definition {CanonicalName}", canonicalName);
                throw;
            }
        }

        public async Task UpdateDefinitionAsync(
            Guid actorUserId,
            Guid definitionId,
            string canonicalName,
            AdditionalPropertyDataTypeEnum dataType,
            string? description,
            Guid? dataEnumeratorId,
            Guid? defaultGroupId,
            bool isActive,
            CancellationToken ct = default
        )
        {
            try
            {
                UserAdditionalPropertyDefinitionEntityModel definition = await _db.WithinTenancy_UserAdditionalProperty_Definitions
                    .FirstOrDefaultAsync(d => d.Id == definitionId, ct)
                    ?? throw new KeyNotFoundException($"User additional property definition {definitionId} not found");

                canonicalName = ValidateCanonicalName(canonicalName);
                await ValidateReferencesAsync(dataType, dataEnumeratorId, defaultGroupId, ct);

                if (definition.IsBuiltIn && (definition.CanonicalName != canonicalName || definition.DataType != dataType))
                {
                    throw new InvalidOperationException("The canonical name and data type of a built-in definition can't be changed");
                }

                if (
                    definition.CanonicalName != canonicalName
                    && await _db.WithinTenancy_UserAdditionalProperty_Definitions.AnyAsync(d => d.Id != definitionId && d.CanonicalName == canonicalName, ct)
                )
                {
                    throw new InvalidOperationException($"A user additional property definition named '{canonicalName}' already exists");
                }

                definition.CanonicalName = canonicalName;
                definition.Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
                definition.DataType = dataType;
                definition.DataEnumeratorId = dataEnumeratorId;
                definition.DefaultGroupId = defaultGroupId;
                definition.IsActive = isActive;
                definition.LastUpdatedAtUtc = DateTime.UtcNow;
                definition.LastUpdatedByUserId = actorUserId;

                await _db.SaveChangesAsync(ct);

                _logger.LogInformation("Updated user additional property definition {DefinitionId} ({CanonicalName})", definitionId, canonicalName);
            }
            catch (Exception ex) when (ex is not KeyNotFoundException and not ArgumentException and not InvalidOperationException)
            {
                _logger.LogError(ex, "Failed to update user additional property definition {DefinitionId}", definitionId);
                throw;
            }
        }

        public async Task ReorderDefinitionsAsync(Guid actorUserId, IReadOnlyList<Guid> orderedDefinitionIds, CancellationToken ct = default)
        {
            try
            {
                if (orderedDefinitionIds.Distinct().Count() != orderedDefinitionIds.Count)
                {
                    throw new ArgumentException("The definition ordering contains duplicate ids", nameof(orderedDefinitionIds));
                }

                List<UserAdditionalPropertyDefinitionEntityModel> definitions = await _db.WithinTenancy_UserAdditionalProperty_Definitions.ToListAsync(ct);

                if (definitions.Count != orderedDefinitionIds.Count || definitions.Any(d => !orderedDefinitionIds.Contains(d.Id)))
                {
                    throw new ArgumentException("The definition ordering must contain every existing definition exactly once", nameof(orderedDefinitionIds));
                }

                DateTime now = DateTime.UtcNow;
                Dictionary<Guid, UserAdditionalPropertyDefinitionEntityModel> definitionsById = definitions.ToDictionary(d => d.Id);

                for (var i = 0; i < orderedDefinitionIds.Count; i++)
                {
                    UserAdditionalPropertyDefinitionEntityModel definition = definitionsById[orderedDefinitionIds[i]];
                    if (definition.SortOrder == i)
                    {
                        continue;
                    }

                    definition.SortOrder = i;
                    definition.LastUpdatedAtUtc = now;
                    definition.LastUpdatedByUserId = actorUserId;
                }

                await _db.SaveChangesAsync(ct);

                _logger.LogInformation("Reordered {Count} user additional property definitions", definitions.Count);
            }
            catch (Exception ex) when (ex is not ArgumentException)
            {
                _logger.LogError(ex, "Failed to reorder user additional property definitions");
                throw;
            }
        }

        public async Task<int> DeleteDefinitionAsync(Guid actorUserId, Guid definitionId, CancellationToken ct = default)
        {
            try
            {
                UserAdditionalPropertyDefinitionEntityModel definition = await _db.WithinTenancy_UserAdditionalProperty_Definitions
                    .Include(d => d.Translations)
                    .Include(d => d.Aliases)
                    .FirstOrDefaultAsync(d => d.Id == definitionId, ct)
                    ?? throw new KeyNotFoundException($"User additional property definition {definitionId} not found");

                if (definition.IsBuiltIn)
                {
                    throw new InvalidOperationException("Built-in definitions can't be deleted; deactivate it instead");
                }

                DateTime now = DateTime.UtcNow;
                List<UserAdditionalPropertyEntityModel> properties = await _db.WithinTenancy_UserAdditionalProperties
                    .Where(p => p.DefinitionId == definitionId)
                    .ToListAsync(ct);

                foreach (UserAdditionalPropertyEntityModel property in properties)
                {
                    property.DefinitionId = null;
                    property.LastUpdatedAtUtc = now;
                    property.LastUpdatedByUserId = actorUserId;
                }

                _db.WithinTenancy_UserAdditionalProperty_Definitions.Remove(definition);
                await _db.SaveChangesAsync(ct);

                _logger.LogInformation(
                    "Deleted user additional property definition {DefinitionId} ({CanonicalName}), unlinking {Count} properties",
                    definitionId, definition.CanonicalName, properties.Count
                );

                return properties.Count;
            }
            catch (Exception ex) when (ex is not KeyNotFoundException and not InvalidOperationException)
            {
                _logger.LogError(ex, "Failed to delete user additional property definition {DefinitionId}", definitionId);
                throw;
            }
        }
        #endregion
        #region ========================= TRANSLATION OPERATIONS =========================
        public async Task SetTranslationAsync(Guid actorUserId, Guid definitionId, string languageCode, string translation, CancellationToken ct = default)
        {
            try
            {
                languageCode = (languageCode ?? string.Empty).Trim();
                translation = (translation ?? string.Empty).Trim();
                if (languageCode.Length == 0)
                {
                    throw new ArgumentException("A language code is required", nameof(languageCode));
                }
                if (translation.Length == 0)
                {
                    throw new ArgumentException("A translation is required", nameof(translation));
                }

                UserAdditionalPropertyDefinitionEntityModel definition = await _db.WithinTenancy_UserAdditionalProperty_Definitions
                    .FirstOrDefaultAsync(d => d.Id == definitionId, ct)
                    ?? throw new KeyNotFoundException($"User additional property definition {definitionId} not found");

                DateTime now = DateTime.UtcNow;

                UserAdditionalPropertyDefinitionTranslationEntityModel existing = await _db.WithinTenancy_UserAdditionalProperty_Definition_Translations
                    .FirstOrDefaultAsync(t => t.UserAdditionalPropertyDefinitionId == definitionId && t.LanguageCode == languageCode, ct);

                if (existing == null)
                {
                    _db.WithinTenancy_UserAdditionalProperty_Definition_Translations.Add(new UserAdditionalPropertyDefinitionTranslationEntityModel
                    {
                        Id = UUIDUtilities.GenerateV5(DatabaseObjectTypeEnum.WithinTenancy_User_AdditionalProperty_Definition_Translation),
                        UserAdditionalPropertyDefinitionId = definitionId,
                        LanguageCode = languageCode,
                        Translation = translation,
                        CreatedByUserId = actorUserId,
                        CreatedAtUtc = now,
                    });
                }
                else
                {
                    if (existing.Translation == translation)
                    {
                        return;
                    }

                    existing.Translation = translation;
                    existing.LastUpdatedAtUtc = now;
                    existing.LastUpdatedByUserId = actorUserId;
                }

                definition.LastUpdatedAtUtc = now;
                definition.LastUpdatedByUserId = actorUserId;

                await _db.SaveChangesAsync(ct);

                _logger.LogInformation("Set {LanguageCode} translation for user additional property definition {DefinitionId}", languageCode, definitionId);
            }
            catch (Exception ex) when (ex is not KeyNotFoundException and not ArgumentException)
            {
                _logger.LogError(ex, "Failed to set {LanguageCode} translation for user additional property definition {DefinitionId}", languageCode, definitionId);
                throw;
            }
        }

        public async Task RemoveTranslationAsync(Guid actorUserId, Guid definitionId, string languageCode, CancellationToken ct = default)
        {
            try
            {
                UserAdditionalPropertyDefinitionEntityModel definition = await _db.WithinTenancy_UserAdditionalProperty_Definitions
                    .FirstOrDefaultAsync(d => d.Id == definitionId, ct)
                    ?? throw new KeyNotFoundException($"User additional property definition {definitionId} not found");

                UserAdditionalPropertyDefinitionTranslationEntityModel translation = await _db.WithinTenancy_UserAdditionalProperty_Definition_Translations
                    .FirstOrDefaultAsync(t => t.UserAdditionalPropertyDefinitionId == definitionId && t.LanguageCode == languageCode, ct)
                    ?? throw new KeyNotFoundException($"No {languageCode} translation for user additional property definition {definitionId}");

                _db.WithinTenancy_UserAdditionalProperty_Definition_Translations.Remove(translation);

                definition.LastUpdatedAtUtc = DateTime.UtcNow;
                definition.LastUpdatedByUserId = actorUserId;

                await _db.SaveChangesAsync(ct);

                _logger.LogInformation("Removed {LanguageCode} translation from user additional property definition {DefinitionId}", languageCode, definitionId);
            }
            catch (Exception ex) when (ex is not KeyNotFoundException and not ArgumentException)
            {
                _logger.LogError(ex, "Failed to remove {LanguageCode} translation from user additional property definition {DefinitionId}", languageCode, definitionId);
                throw;
            }
        }
        #endregion
        #region ========================= ALIAS OPERATIONS =========================
        public async Task<(UserAdditionalPropertyDefinitionAliasEntityModel Alias, int LinkedPropertyCount)> AddAliasAsync(
            Guid actorUserId,
            Guid definitionId,
            string alias,
            CancellationToken ct = default
        )
        {
            try
            {
                alias = (alias ?? string.Empty).Trim();
                string normalisedAlias = PropertyNameHandlingUtilities.NormaliseForMatching(alias);
                if (normalisedAlias.Length == 0)
                    throw new ArgumentException("The alias must contain at least one letter or digit", nameof(alias));

                UserAdditionalPropertyDefinitionEntityModel definition = await _db.WithinTenancy_UserAdditionalProperty_Definitions
                    .FirstOrDefaultAsync(d => d.Id == definitionId, ct)
                    ?? throw new KeyNotFoundException($"User additional property definition {definitionId} not found");

                UserAdditionalPropertyDefinitionAliasEntityModel existing = await _db.WithinTenancy_UserAdditionalProperty_Definition_Aliases
                    .Include(a => a.UserAdditionalPropertyDefinition)
                    .FirstOrDefaultAsync(a => a.NormalisedAlias == normalisedAlias, ct);

                DateTime now = DateTime.UtcNow;
                UserAdditionalPropertyDefinitionAliasEntityModel result;

                if (existing != null)
                {
                    if (existing.UserAdditionalPropertyDefinitionId != definitionId)
                        throw new InvalidOperationException($"'{alias}' already resolves to '{existing.UserAdditionalPropertyDefinition?.CanonicalName}'");

                    result = existing;
                }
                else
                {
                    result = NewAlias(definitionId, alias, normalisedAlias, actorUserId, now);
                    _db.WithinTenancy_UserAdditionalProperty_Definition_Aliases.Add(result);

                    definition.LastUpdatedAtUtc = now;
                    definition.LastUpdatedByUserId = actorUserId;
                }

                int linked = await LinkUnlinkedPropertiesAsync(definitionId, new HashSet<string> { normalisedAlias }, actorUserId, now, ct);

                await _db.SaveChangesAsync(ct);

                _logger.LogInformation("Added alias {Alias} to user additional property definition {DefinitionId}, linking {Count} properties",
                    alias, definitionId, linked);

                return (result, linked);
            }
            catch (Exception ex) when (ex is not KeyNotFoundException and not ArgumentException and not InvalidOperationException)
            {
                _logger.LogError(ex, "Failed to add alias {Alias} to user additional property definition {DefinitionId}", alias, definitionId);
                throw;
            }
        }

        public async Task RemoveAliasAsync(Guid actorUserId, Guid definitionId, Guid aliasId, CancellationToken ct = default)
        {
            try
            {
                UserAdditionalPropertyDefinitionEntityModel definition = await _db.WithinTenancy_UserAdditionalProperty_Definitions
                    .FirstOrDefaultAsync(d => d.Id == definitionId, ct)
                    ?? throw new KeyNotFoundException($"User additional property definition {definitionId} not found");

                UserAdditionalPropertyDefinitionAliasEntityModel alias = await _db.WithinTenancy_UserAdditionalProperty_Definition_Aliases
                    .FirstOrDefaultAsync(a => a.Id == aliasId && a.UserAdditionalPropertyDefinitionId == definitionId, ct)
                    ?? throw new KeyNotFoundException($"Alias {aliasId} not found on user additional property definition {definitionId}");

                _db.WithinTenancy_UserAdditionalProperty_Definition_Aliases.Remove(alias);

                definition.LastUpdatedAtUtc = DateTime.UtcNow;
                definition.LastUpdatedByUserId = actorUserId;

                await _db.SaveChangesAsync(ct);

                _logger.LogInformation("Removed alias {Alias} from user additional property definition {DefinitionId}", alias.Alias, definitionId);
            }
            catch (Exception ex) when (ex is not KeyNotFoundException)
            {
                _logger.LogError(ex, "Failed to remove alias {AliasId} from user additional property definition {DefinitionId}", aliasId, definitionId);
                throw;
            }
        }
        #endregion
        #region ========================= SEEDING =========================
        public async Task<AdditionalPropertyDefinitionSeedResultDTO> SeedBuiltInDefinitionsAsync(Guid? actorUserId, CancellationToken ct = default)
        {
            try
            {
                AdditionalPropertyDefinitionSeedResultDTO result = new();
                DateTime now = DateTime.UtcNow;

                List<UserAdditionalPropertyDefinitionEntityModel> definitions = await _db.WithinTenancy_UserAdditionalProperty_Definitions
                    .Include(d => d.Aliases)
                    .ToListAsync(ct);
                Dictionary<string, UserAdditionalPropertyDefinitionEntityModel> definitionsByName = definitions.ToDictionary(d => d.CanonicalName);

                Dictionary<string, Guid> takenAliases = await _db.WithinTenancy_UserAdditionalProperty_Definition_Aliases
                    .ToDictionaryAsync(a => a.NormalisedAlias, a => a.UserAdditionalPropertyDefinitionId, ct);

                int nextSortOrder = definitions.Count == 0 ? 0 : definitions.Max(d => d.SortOrder) + 1;
                Dictionary<Guid, HashSet<string>> keysByDefinition = new();

                foreach (BuiltInAdditionalPropertyDefinitionCatalogue.Entry entry in BuiltInAdditionalPropertyDefinitionCatalogue.User)
                {
                    if (!definitionsByName.TryGetValue(entry.CanonicalName, out var definition))
                    {
                        definition = new UserAdditionalPropertyDefinitionEntityModel
                        {
                            Id = UUIDUtilities.GenerateV5(DatabaseObjectTypeEnum.WithinTenancy_User_AdditionalProperty_Definition),
                            CanonicalName = entry.CanonicalName,
                            Description = entry.Description,
                            DataType = entry.DataType,
                            IsBuiltIn = true,
                            IsActive = true,
                            SortOrder = nextSortOrder++,
                            CreatedByUserId = actorUserId,
                            CreatedAtUtc = now,
                            Translations = new List<UserAdditionalPropertyDefinitionTranslationEntityModel>(),
                            Aliases = new List<UserAdditionalPropertyDefinitionAliasEntityModel>(),
                        };
                        AddTranslations(definition, entry.Translations, actorUserId, now);

                        _db.WithinTenancy_UserAdditionalProperty_Definitions.Add(definition);
                        definitionsByName[entry.CanonicalName] = definition;
                        result.CreatedDefinitionCount++;
                    }

                    HashSet<string> keys = keysByDefinition.TryGetValue(definition.Id, out var existingKeys)
                        ? existingKeys
                        : keysByDefinition[definition.Id] = new HashSet<string>();

                    foreach ((string normalisedAlias, string alias) in NormaliseAliases(entry.Aliases))
                    {
                        if (takenAliases.TryGetValue(normalisedAlias, out var ownerId))
                        {
                            if (ownerId == definition.Id)
                            {
                                keys.Add(normalisedAlias);
                            }
                            else
                            {
                                result.SkippedAliases.Add(alias);
                            }
                            continue;
                        }

                        definition.Aliases ??= new List<UserAdditionalPropertyDefinitionAliasEntityModel>();
                        definition.Aliases.Add(NewAlias(definition.Id, alias, normalisedAlias, actorUserId, now));
                        takenAliases[normalisedAlias] = definition.Id;
                        keys.Add(normalisedAlias);
                        result.AddedAliasCount++;
                    }
                }

                foreach (var (definitionId, keys) in keysByDefinition)
                {
                    result.LinkedPropertyCount += await LinkUnlinkedPropertiesAsync(definitionId, keys, actorUserId, now, ct);
                }

                await _db.SaveChangesAsync(ct);

                _logger.LogInformation(
                    "Seeded built-in user additional property definitions: {Created} created, {Aliases} aliases added, {Skipped} aliases skipped, {Linked} properties linked",
                    result.CreatedDefinitionCount, result.AddedAliasCount, result.SkippedAliases.Count, result.LinkedPropertyCount
                );

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to seed built-in user additional property definitions");
                throw;
            }
        }
        #endregion
        #region ========================= HELPERS =========================
        private static string ValidateCanonicalName(string canonicalName)
        {
            canonicalName = (canonicalName ?? string.Empty).Trim();

            if (canonicalName.Length == 0)
            {
                throw new ArgumentException("A canonical name is required", nameof(canonicalName));
            }

            return canonicalName;
        }

        private async Task ValidateReferencesAsync(
            AdditionalPropertyDataTypeEnum dataType,
            Guid? dataEnumeratorId,
            Guid? defaultGroupId,
            CancellationToken ct
        )
        {
            if (dataType == AdditionalPropertyDataTypeEnum.DataEnumeratorValue)
            {
                if (dataEnumeratorId == null)
                {
                    throw new ArgumentException("A data enumerator is required for the DataEnumeratorValue data type", nameof(dataEnumeratorId));
                }

                if (!await _db.WithinTenancy_DataEnumerator_Enumerators.AnyAsync(e => e.Id == dataEnumeratorId && e.Scope == DataEnumeratorScopeEnum.AdditionalProperty, ct))
                {
                    throw new ArgumentException($"Data enumerator {dataEnumeratorId} not found, or isn't an additional property enumerator", nameof(dataEnumeratorId));
                }
            }
            else if (dataEnumeratorId != null)
            {
                throw new ArgumentException("A data enumerator can only be set for the DataEnumeratorValue data type", nameof(dataEnumeratorId));
            }

            if (defaultGroupId != null && !await _db.WithinTenancy_UserAdditionalProperty_Groups.AnyAsync(g => g.Id == defaultGroupId, ct))
            {
                throw new ArgumentException($"User additional property group {defaultGroupId} not found", nameof(defaultGroupId));
            }
        }

        private static Dictionary<string, string> NormaliseAliases(IEnumerable<string>? aliases)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            if (aliases == null)
            {
                return result;
            }

            foreach (string raw in aliases)
            {
                string alias = (raw ?? string.Empty).Trim();
                string key = PropertyNameHandlingUtilities.NormaliseForMatching(alias);
                if (key.Length > 0)
                {
                    result.TryAdd(key, alias);
                }
            }

            return result;
        }

        private async Task EnsureAliasesAvailableAsync(IEnumerable<string> normalisedAliases, Guid? exceptDefinitionId, CancellationToken ct)
        {
            List<string> keys = normalisedAliases.ToList();
            if (keys.Count == 0) return;

            UserAdditionalPropertyDefinitionAliasEntityModel clash = await _db.WithinTenancy_UserAdditionalProperty_Definition_Aliases
                .Include(a => a.UserAdditionalPropertyDefinition)
                .Where(a => keys.Contains(a.NormalisedAlias) && a.UserAdditionalPropertyDefinitionId != exceptDefinitionId)
                .FirstOrDefaultAsync(ct);

            if (clash != null)
            {
                throw new InvalidOperationException($"'{clash.Alias}' already resolves to '{clash.UserAdditionalPropertyDefinition?.CanonicalName}'");
            }
        }

        private static void AddTranslations(
            UserAdditionalPropertyDefinitionEntityModel definition,
            IReadOnlyDictionary<string, string>? translations,
            Guid? actorUserId,
            DateTime now
        )
        {
            if (translations == null)
            {
                return;
            }
            definition.Translations ??= new List<UserAdditionalPropertyDefinitionTranslationEntityModel>();

            foreach ((string languageCode, string translation) in translations)
            {
                if (string.IsNullOrWhiteSpace(languageCode) || string.IsNullOrWhiteSpace(translation))
                {
                    continue;
                }

                definition.Translations.Add(new UserAdditionalPropertyDefinitionTranslationEntityModel
                {
                    Id = UUIDUtilities.GenerateV5(DatabaseObjectTypeEnum.WithinTenancy_User_AdditionalProperty_Definition_Translation),
                    UserAdditionalPropertyDefinitionId = definition.Id,
                    LanguageCode = languageCode.Trim(),
                    Translation = translation.Trim(),
                    CreatedByUserId = actorUserId,
                    CreatedAtUtc = now,
                });
            }
        }

        private static UserAdditionalPropertyDefinitionAliasEntityModel NewAlias(
            Guid definitionId,
            string alias,
            string normalisedAlias,
            Guid? actorUserId,
            DateTime now
        )
        {
            return new UserAdditionalPropertyDefinitionAliasEntityModel
            {
                Id = UUIDUtilities.GenerateV5(DatabaseObjectTypeEnum.WithinTenancy_User_AdditionalProperty_Definition_Alias),
                UserAdditionalPropertyDefinitionId = definitionId,
                Alias = alias,
                NormalisedAlias = normalisedAlias,
                CreatedByUserId = actorUserId,
                CreatedAtUtc = now,
            };
        }

        private async Task<int> LinkUnlinkedPropertiesAsync(
            Guid definitionId,
            IReadOnlySet<string> normalisedKeys,
            Guid? actorUserId,
            DateTime now,
            CancellationToken ct
        )
        {
            if (normalisedKeys.Count == 0)
            {
                return 0;
            }

            var candidates = await _db.WithinTenancy_UserAdditionalProperties
                .Where(p => p.DefinitionId == null)
                .Select(p => new { p.Id, p.DisplayName })
                .ToListAsync(ct);

            List<Guid> matchingIds = candidates
                .Where(c => normalisedKeys.Contains(PropertyNameHandlingUtilities.NormaliseForMatching(c.DisplayName)))
                .Select(c => c.Id)
                .ToList();

            foreach (Guid[] batch in matchingIds.Chunk(LinkBatchSize))
            {
                List<UserAdditionalPropertyEntityModel> properties = await _db.WithinTenancy_UserAdditionalProperties
                    .Where(p => batch.Contains(p.Id))
                    .ToListAsync(ct);

                foreach (UserAdditionalPropertyEntityModel property in properties)
                {
                    property.DefinitionId = definitionId;
                    property.LastUpdatedAtUtc = now;
                    property.LastUpdatedByUserId = actorUserId;
                }
            }

            return matchingIds.Count;
        }
        #endregion
    }
}
