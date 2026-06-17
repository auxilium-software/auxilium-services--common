using AuxiliumSoftware.AuxiliumServices.Common.DataTransferObjects;
using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework;
using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels;
using AuxiliumSoftware.AuxiliumServices.Common.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AuxiliumSoftware.AuxiliumServices.Common.Services.Implementations;

public class DataEnumeratorService : IDataEnumeratorService
{
    private readonly AuxiliumDbContext _db;
    private readonly ILogger<DataEnumeratorService> _logger;

    public DataEnumeratorService(
        AuxiliumDbContext db,
        ILogger<DataEnumeratorService> logger)
    {
        _db = db;
        _logger = logger;
    }

    #region ========================= ENUMERATOR TYPE OPERATIONS =========================
    public async Task<List<DataEnumeratorEntityModel>> GetAllEnumeratorsAsync(
        bool includeInactive = false,
        CancellationToken ct = default)
    {
        try
        {
            var query = _db.DataEnumerator_Enumerators.AsQueryable();

            if (!includeInactive)
                query = query.Where(e => e.IsActive);

            return await query
                .OrderBy(e => e.CanonicalName)
                .ToListAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get enumerators");
            throw;
        }
    }

    public async Task<DataEnumeratorEntityModel?> GetEnumeratorAsync(
        Guid id,
        CancellationToken ct = default)
    {
        try
        {
            return await _db.DataEnumerator_Enumerators
                .Include(e => e.EnumeratorValues.OrderBy(v => v.SortOrder))
                .FirstOrDefaultAsync(e => e.Id == id, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get enumerator {EnumeratorId}", id);
            return null;
        }
    }

    public async Task<DataEnumeratorEntityModel?> GetEnumeratorByNameAsync(
        string name,
        bool activeValuesOnly = true,
        CancellationToken ct = default)
    {
        try
        {
            var query = _db.DataEnumerator_Enumerators
                .Include(e => e.EnumeratorValues.OrderBy(v => v.SortOrder))
                .Where(e => e.CanonicalName == name && e.IsActive);

            var enumerator = await query.FirstOrDefaultAsync(ct);

            if (enumerator != null && activeValuesOnly)
                enumerator.EnumeratorValues = enumerator.EnumeratorValues.Where(v => v.IsActive).ToList();

            return enumerator;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get enumerator by name {Name}", name);
            return null;
        }
    }

    public async Task<DataEnumeratorEntityModel> CreateEnumeratorAsync(
        string name,
        string? description,
        UserEntityModel createdBy,
        CancellationToken ct = default)
    {
        try
        {
            var enumerator = new DataEnumeratorEntityModel
            {
                Id = Guid.NewGuid(),
                CreatedAtUtc = DateTime.UtcNow,
                CreatedByUserId = createdBy.Id,
                CanonicalName = name,
                Description = description,
                IsActive = true,
            };

            _db.DataEnumerator_Enumerators.Add(enumerator);
            await _db.SaveChangesAsync(ct);

            _logger.LogInformation(
                "Created enumerator {EnumeratorId} ({Name}) by user {UserId}",
                enumerator.Id, name, createdBy.Id);

            return enumerator;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create enumerator {Name}", name);
            throw;
        }
    }

    public async Task<DataEnumeratorEntityModel> UpdateEnumeratorAsync(
        Guid id,
        string? name,
        string? description,
        UserEntityModel updatedBy,
        CancellationToken ct = default)
    {
        try
        {
            var enumerator = await _db.DataEnumerator_Enumerators.FindAsync([id], ct)
                ?? throw new KeyNotFoundException($"Enumerator {id} not found");

            if (name != null) enumerator.CanonicalName = name;
            if (description != null) enumerator.Description = description;
            enumerator.LastUpdatedAtUtc = DateTime.UtcNow;
            enumerator.LastUpdatedByUserId = updatedBy.Id;

            await _db.SaveChangesAsync(ct);

            _logger.LogInformation(
                "Updated enumerator {EnumeratorId} by user {UserId}",
                id, updatedBy.Id);

            return enumerator;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update enumerator {EnumeratorId}", id);
            throw;
        }
    }

    public async Task SetEnumeratorActiveAsync(
        Guid id,
        bool isActive,
        UserEntityModel updatedBy,
        CancellationToken ct = default)
    {
        try
        {
            var enumerator = await _db.DataEnumerator_Enumerators.FindAsync([id], ct)
                ?? throw new KeyNotFoundException($"Enumerator {id} not found");

            enumerator.IsActive = isActive;
            enumerator.LastUpdatedAtUtc = DateTime.UtcNow;
            enumerator.LastUpdatedByUserId = updatedBy.Id;

            await _db.SaveChangesAsync(ct);

            _logger.LogInformation(
                "Enumerator {EnumeratorId} set IsActive={IsActive} by user {UserId}",
                id, isActive, updatedBy.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set active state on enumerator {EnumeratorId}", id);
            throw;
        }
    }
    #endregion

    #region ========================= ENUMERATOR TYPE TRANSLATION OPERATIONS =========================
    public async Task<List<DataEnumeratorTranslationEntityModel>> GetEnumeratorTranslationsAsync(
        Guid enumeratorId,
        CancellationToken ct = default)
    {
        try
        {
            return await _db.DataEnumerator_EnumeratorTranslations
                .Where(t => t.DataEnumeratorId == enumeratorId)
                .OrderBy(t => t.LanguageCode)
                .ToListAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get translations for enumerator {EnumeratorId}", enumeratorId);
            throw;
        }
    }

    public async Task<DataEnumeratorTranslationEntityModel> CreateEnumeratorTranslationAsync(
        Guid enumeratorId,
        string languageCode,
        string translation,
        UserEntityModel createdBy,
        CancellationToken ct = default)
    {
        try
        {
            var enumeratorExists = await _db.DataEnumerator_Enumerators
                .AnyAsync(e => e.Id == enumeratorId, ct);

            if (!enumeratorExists)
                throw new KeyNotFoundException($"Enumerator {enumeratorId} not found");

            var lang = EnumValueUtilities.NormaliseLanguageCode(languageCode);

            // one translation per (enumerator, language) - checking it here instead of getting a duplicate key error
            var alreadyExists = await _db.DataEnumerator_EnumeratorTranslations
                .AnyAsync(t => t.DataEnumeratorId == enumeratorId && t.LanguageCode == lang, ct);

            if (alreadyExists)
                throw new InvalidOperationException(
                    $"Enumerator {enumeratorId} already has a translation for '{lang}'");

            var entity = new DataEnumeratorTranslationEntityModel
            {
                Id = Guid.NewGuid(),
                CreatedAtUtc = DateTime.UtcNow,
                CreatedByUserId = createdBy.Id,
                DataEnumeratorId = enumeratorId,
                LanguageCode = lang,
                Translation = translation,
            };

            _db.DataEnumerator_EnumeratorTranslations.Add(entity);
            await _db.SaveChangesAsync(ct);

            _logger.LogInformation(
                "Created enumerator translation {TranslationId} ({Language}) for enumerator {EnumeratorId} by user {UserId}",
                entity.Id, lang, enumeratorId, createdBy.Id);

            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create translation for enumerator {EnumeratorId}", enumeratorId);
            throw;
        }
    }

    public async Task<DataEnumeratorTranslationEntityModel> UpdateEnumeratorTranslationAsync(
        Guid translationId,
        string? languageCode,
        string? translation,
        UserEntityModel updatedBy,
        CancellationToken ct = default)
    {
        try
        {
            var entity = await _db.DataEnumerator_EnumeratorTranslations.FindAsync([translationId], ct)
                ?? throw new KeyNotFoundException($"Translation {translationId} not found");

            if (languageCode != null)
            {
                var lang = EnumValueUtilities.NormaliseLanguageCode(languageCode);

                // only guard the unique (enumerator, language) index if the language actually changes
                if (lang != entity.LanguageCode)
                {
                    var clash = await _db.DataEnumerator_EnumeratorTranslations
                        .AnyAsync(t => t.DataEnumeratorId == entity.DataEnumeratorId
                                    && t.LanguageCode == lang
                                    && t.Id != translationId, ct);

                    if (clash)
                        throw new InvalidOperationException(
                            $"Enumerator {entity.DataEnumeratorId} already has a translation for '{lang}'");

                    entity.LanguageCode = lang;
                }
            }

            if (translation != null) entity.Translation = translation;

            entity.LastUpdatedAtUtc = DateTime.UtcNow;
            entity.LastUpdatedByUserId = updatedBy.Id;

            await _db.SaveChangesAsync(ct);

            _logger.LogInformation(
                "Updated enumerator translation {TranslationId} by user {UserId}",
                translationId, updatedBy.Id);

            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update enumerator translation {TranslationId}", translationId);
            throw;
        }
    }

    public async Task DeleteEnumeratorTranslationAsync(
        Guid translationId,
        CancellationToken ct = default)
    {
        try
        {
            var entity = await _db.DataEnumerator_EnumeratorTranslations.FindAsync([translationId], ct)
                ?? throw new KeyNotFoundException($"Translation {translationId} not found");

            _db.DataEnumerator_EnumeratorTranslations.Remove(entity);
            await _db.SaveChangesAsync(ct);

            _logger.LogInformation("Deleted enumerator translation {TranslationId}", translationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete enumerator translation {TranslationId}", translationId);
            throw;
        }
    }
    #endregion

    #region ========================= ENUMERATOR VALUE OPERATIONS =========================
    public async Task<List<DataEnumeratorValueEntityModel>> GetValuesAsync(
        Guid enumeratorId,
        bool includeInactive = false,
        CancellationToken ct = default)
    {
        try
        {
            var query = _db.DataEnumerator_EnumeratorValues
                .Where(v => v.EnumTypeId == enumeratorId);

            if (!includeInactive)
                query = query.Where(v => v.IsActive);

            return await query
                .OrderBy(v => v.SortOrder)
                .ToListAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get values for enumerator {EnumeratorId}", enumeratorId);
            throw;
        }
    }

    public async Task<DataEnumeratorValueEntityModel?> GetValueAsync(
        Guid valueId,
        CancellationToken ct = default)
    {
        try
        {
            return await _db.DataEnumerator_EnumeratorValues
                .Include(v => v.EnumType)
                .FirstOrDefaultAsync(v => v.Id == valueId, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get enumerator value {ValueId}", valueId);
            return null;
        }
    }

    public async Task<DataEnumeratorValueEntityModel> CreateValueAsync(
        Guid enumeratorId,
        string canonicalName,
        UserEntityModel createdBy,
        int? sortOrder = null,
        CancellationToken ct = default)
    {
        try
        {
            var enumeratorExists = await _db.DataEnumerator_Enumerators
                .AnyAsync(e => e.Id == enumeratorId, ct);

            if (!enumeratorExists)
                throw new KeyNotFoundException($"Enumerator {enumeratorId} not found");

            // one value per (enumerator, canonical name) - clean error instead of a raw 1062
            var alreadyExists = await _db.DataEnumerator_EnumeratorValues
                .AnyAsync(v => v.EnumTypeId == enumeratorId && v.CanonicalName == canonicalName, ct);

            if (alreadyExists)
                throw new InvalidOperationException(
                    $"A value named '{canonicalName}' already exists under enumerator {enumeratorId}");

            // default sort order to end of list
            if (sortOrder == null)
            {
                var maxSortOrder = await _db.DataEnumerator_EnumeratorValues
                    .Where(v => v.EnumTypeId == enumeratorId)
                    .MaxAsync(v => (int?)v.SortOrder, ct);

                sortOrder = (maxSortOrder ?? -1) + 1;
            }

            var value = new DataEnumeratorValueEntityModel
            {
                Id = Guid.NewGuid(),
                CreatedAtUtc = DateTime.UtcNow,
                CreatedByUserId = createdBy.Id,
                EnumTypeId = enumeratorId,
                CanonicalName = canonicalName,
                IsActive = true,
                SortOrder = sortOrder.Value,
            };

            _db.DataEnumerator_EnumeratorValues.Add(value);
            await _db.SaveChangesAsync(ct);

            _logger.LogInformation(
                "Created enumerator value {ValueId} ({CanonicalName}) under enumerator {EnumeratorId} by user {UserId}",
                value.Id, canonicalName, enumeratorId, createdBy.Id);

            return value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create value under enumerator {EnumeratorId}", enumeratorId);
            throw;
        }
    }

    public async Task<DataEnumeratorValueEntityModel> UpdateValueAsync(
        Guid valueId,
        string? canonicalName,
        UserEntityModel updatedBy,
        CancellationToken ct = default)
    {
        try
        {
            var value = await _db.DataEnumerator_EnumeratorValues.FindAsync([valueId], ct)
                ?? throw new KeyNotFoundException($"Enumerator value {valueId} not found");

            if (canonicalName != null && canonicalName != value.CanonicalName)
            {
                // guard the unique (enumerator, canonical name) index on rename
                var clash = await _db.DataEnumerator_EnumeratorValues
                    .AnyAsync(v => v.EnumTypeId == value.EnumTypeId
                                && v.CanonicalName == canonicalName
                                && v.Id != valueId, ct);

                if (clash)
                    throw new InvalidOperationException(
                        $"A value named '{canonicalName}' already exists under enumerator {value.EnumTypeId}");

                value.CanonicalName = canonicalName;
            }

            value.LastUpdatedAtUtc = DateTime.UtcNow;
            value.LastUpdatedByUserId = updatedBy.Id;

            await _db.SaveChangesAsync(ct);

            _logger.LogInformation(
                "Updated enumerator value {ValueId} by user {UserId}",
                valueId, updatedBy.Id);

            return value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update enumerator value {ValueId}", valueId);
            throw;
        }
    }

    public async Task SetValueActiveAsync(
        Guid valueId,
        bool isActive,
        UserEntityModel updatedBy,
        CancellationToken ct = default)
    {
        try
        {
            var value = await _db.DataEnumerator_EnumeratorValues.FindAsync([valueId], ct)
                ?? throw new KeyNotFoundException($"Enumerator value {valueId} not found");

            value.IsActive = isActive;
            value.LastUpdatedAtUtc = DateTime.UtcNow;
            value.LastUpdatedByUserId = updatedBy.Id;

            await _db.SaveChangesAsync(ct);

            _logger.LogInformation(
                "Enumerator value {ValueId} set IsActive={IsActive} by user {UserId}",
                valueId, isActive, updatedBy.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set active state on enumerator value {ValueId}", valueId);
            throw;
        }
    }

    public async Task ReorderValuesAsync(
        Guid enumeratorId,
        List<Guid> orderedValueIds,
        UserEntityModel updatedBy,
        CancellationToken ct = default)
    {
        try
        {
            var values = await _db.DataEnumerator_EnumeratorValues
                .Where(v => v.EnumTypeId == enumeratorId && orderedValueIds.Contains(v.Id))
                .ToListAsync(ct);

            for (int i = 0; i < orderedValueIds.Count; i++)
            {
                var value = values.FirstOrDefault(v => v.Id == orderedValueIds[i]);
                if (value == null) continue;

                value.SortOrder = i;
                value.LastUpdatedAtUtc = DateTime.UtcNow;
                value.LastUpdatedByUserId = updatedBy.Id;
            }

            await _db.SaveChangesAsync(ct);

            _logger.LogInformation(
                "Reordered {Count} values under enumerator {EnumeratorId} by user {UserId}",
                orderedValueIds.Count, enumeratorId, updatedBy.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to reorder values for enumerator {EnumeratorId}", enumeratorId);
            throw;
        }
    }
    #endregion

    #region ========================= TRANSLATION OPERATIONS =========================
    public async Task<List<DataEnumeratorValueTranslationEntityModel>> GetTranslationsAsync(
        Guid valueId,
        CancellationToken ct = default)
    {
        try
        {
            return await _db.DataEnumerator_EnumeratorValueTranslations
                .Where(t => t.DataEnumeratorValueId == valueId)
                .OrderBy(t => t.LanguageCode)
                .ToListAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get translations for value {ValueId}", valueId);
            throw;
        }
    }

    public async Task<DataEnumeratorValueTranslationEntityModel> CreateTranslationAsync(
        Guid valueId,
        string languageCode,
        string translation,
        UserEntityModel createdBy,
        CancellationToken ct = default)
    {
        try
        {
            var valueExists = await _db.DataEnumerator_EnumeratorValues
                .AnyAsync(v => v.Id == valueId, ct);

            if (!valueExists)
                throw new KeyNotFoundException($"Enumerator value {valueId} not found");

            var lang = EnumValueUtilities.NormaliseLanguageCode(languageCode);

            // one translation per (value, language) - checking it here instead of getting a duplicate key error
            var alreadyExists = await _db.DataEnumerator_EnumeratorValueTranslations
                .AnyAsync(t => t.DataEnumeratorValueId == valueId && t.LanguageCode == lang, ct);

            if (alreadyExists)
                throw new InvalidOperationException(
                    $"Value {valueId} already has a translation for '{lang}'");

            var entity = new DataEnumeratorValueTranslationEntityModel
            {
                Id = Guid.NewGuid(),
                CreatedAtUtc = DateTime.UtcNow,
                CreatedByUserId = createdBy.Id,
                DataEnumeratorValueId = valueId,
                LanguageCode = lang,
                Translation = translation,
            };

            _db.DataEnumerator_EnumeratorValueTranslations.Add(entity);
            await _db.SaveChangesAsync(ct);

            _logger.LogInformation(
                "Created translation {TranslationId} ({Language}) for value {ValueId} by user {UserId}",
                entity.Id, lang, valueId, createdBy.Id);

            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create translation for value {ValueId}", valueId);
            throw;
        }
    }

    public async Task<DataEnumeratorValueTranslationEntityModel> UpdateTranslationAsync(
        Guid translationId,
        string? languageCode,
        string? translation,
        UserEntityModel updatedBy,
        CancellationToken ct = default)
    {
        try
        {
            var entity = await _db.DataEnumerator_EnumeratorValueTranslations.FindAsync([translationId], ct)
                ?? throw new KeyNotFoundException($"Translation {translationId} not found");

            if (languageCode != null)
            {
                var lang = EnumValueUtilities.NormaliseLanguageCode(languageCode);

                // only guard the unique (value, language) index if the language actually changes
                if (lang != entity.LanguageCode)
                {
                    var clash = await _db.DataEnumerator_EnumeratorValueTranslations
                        .AnyAsync(t => t.DataEnumeratorValueId == entity.DataEnumeratorValueId
                                    && t.LanguageCode == lang
                                    && t.Id != translationId, ct);

                    if (clash)
                        throw new InvalidOperationException(
                            $"Value {entity.DataEnumeratorValueId} already has a translation for '{lang}'");

                    entity.LanguageCode = lang;
                }
            }

            if (translation != null) entity.Translation = translation;

            entity.LastUpdatedAtUtc = DateTime.UtcNow;
            entity.LastUpdatedByUserId = updatedBy.Id;

            await _db.SaveChangesAsync(ct);

            _logger.LogInformation(
                "Updated translation {TranslationId} by user {UserId}",
                translationId, updatedBy.Id);

            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update translation {TranslationId}", translationId);
            throw;
        }
    }

    public async Task DeleteTranslationAsync(
        Guid translationId,
        CancellationToken ct = default)
    {
        try
        {
            var entity = await _db.DataEnumerator_EnumeratorValueTranslations.FindAsync([translationId], ct)
                ?? throw new KeyNotFoundException($"Translation {translationId} not found");

            _db.DataEnumerator_EnumeratorValueTranslations.Remove(entity);
            await _db.SaveChangesAsync(ct);

            _logger.LogInformation("Deleted translation {TranslationId}", translationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete translation {TranslationId}", translationId);
            throw;
        }
    }
    #endregion


    public async Task<Dictionary<Guid, ResolvedDataEnumeratorValueDTO>> ResolveValueDisplaysAsync(
        IEnumerable<Guid> valueIds,
        string? locale,
        CancellationToken ct = default
    )
    {
        var ids = valueIds.Distinct().ToList();
        var result = new Dictionary<Guid, ResolvedDataEnumeratorValueDTO>();
        if (ids.Count == 0) return result;

        var lang = locale != null ? EnumValueUtilities.NormaliseLanguageCode(locale) : null;

        var values = await _db.DataEnumerator_EnumeratorValues
            .Include(v => v.Translations)
            .Include(v => v.EnumType)
                .ThenInclude(t => t.Translations)
            .Where(v => ids.Contains(v.Id))
            .ToListAsync(ct);

        foreach (var v in values)
        {
            var valueDisplay = v.CanonicalName;
            if (lang != null)
            {
                var tr = v.Translations?.FirstOrDefault(t => t.LanguageCode == lang);
                if (tr != null) valueDisplay = tr.Translation;
            }

            var enumCanonical = v.EnumType?.CanonicalName ?? string.Empty;
            var enumDisplay = enumCanonical;
            if (lang != null && v.EnumType?.Translations != null)
            {
                var et = v.EnumType.Translations.FirstOrDefault(t => t.LanguageCode == lang);
                if (et != null) enumDisplay = et.Translation;
            }

            result[v.Id] = new ResolvedDataEnumeratorValueDTO
            {
                ValueId = v.Id,
                EnumTypeId = v.EnumTypeId,
                ValueCanonicalName = v.CanonicalName,
                ValueDisplay = valueDisplay,
                EnumCanonicalName = enumCanonical,
                EnumDisplay = enumDisplay,
            };
        }

        return result;
    }
}
