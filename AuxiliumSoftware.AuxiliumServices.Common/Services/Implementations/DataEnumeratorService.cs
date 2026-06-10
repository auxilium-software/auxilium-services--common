using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework;
using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels;
using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators;
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
                .OrderBy(e => e.Name)
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
                .Where(e => e.Name == name && e.IsActive);

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
        EnumDataTypeEnum dataType,
        UserEntityModel createdBy,
        CancellationToken ct = default)
    {
        try
        {
            var enumerator = new DataEnumeratorEntityModel
            {
                Id = Guid.NewGuid(),
                CreatedAtUtc = DateTime.UtcNow,
                CreatedBy = createdBy.Id,
                Name = name,
                Description = description,
                DataType = dataType,
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

            if (name != null) enumerator.Name = name;
            if (description != null) enumerator.Description = description;
            enumerator.LastUpdatedAtUtc = DateTime.UtcNow;
            enumerator.LastUpdatedBy = updatedBy.Id;

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
            enumerator.LastUpdatedBy = updatedBy.Id;

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
        string displayName,
        string storedValue,
        UserEntityModel createdBy,
        int? sortOrder = null,
        CancellationToken ct = default)
    {
        try
        {
            var enumerator = await _db.DataEnumerator_Enumerators.FindAsync([enumeratorId], ct)
                ?? throw new KeyNotFoundException($"Enumerator {enumeratorId} not found");

            var canonical = EnumValueUtilities.Canonicalise(storedValue);
            EnumValueUtilities.ValidateAgainstType(canonical, enumerator.DataType);
            var hash = EnumValueUtilities.Hash(canonical);

            var alreadyExists = await _db.DataEnumerator_EnumeratorValues
                .AnyAsync(v => v.EnumTypeId == enumeratorId && v.ValueHash == hash, ct);

            if (alreadyExists)
                throw new InvalidOperationException(
                    $"A value equal to '{canonical}' already exists under enumerator {enumeratorId}");

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
                CreatedBy = createdBy.Id,
                EnumTypeId = enumeratorId,
                DisplayName = displayName,
                EnumValueJson = canonical,
                ValueHash = hash,
                IsActive = true,
                SortOrder = sortOrder.Value,
            };

            _db.DataEnumerator_EnumeratorValues.Add(value);
            await _db.SaveChangesAsync(ct);

            _logger.LogInformation(
                "Created enumerator value {ValueId} ({DisplayName}) under enumerator {EnumeratorId} by user {UserId}",
                value.Id, displayName, enumeratorId, createdBy.Id);

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
        string? displayName,
        string? storedValue,
        UserEntityModel updatedBy,
        CancellationToken ct = default)
    {
        try
        {
            var value = await _db.DataEnumerator_EnumeratorValues.FindAsync([valueId], ct)
                ?? throw new KeyNotFoundException($"Enumerator value {valueId} not found");

            if (displayName != null) value.DisplayName = displayName;
            if (storedValue != null)
            {
                value.EnumValueJson = EnumValueUtilities.Canonicalise(storedValue);
                value.ValueHash = EnumValueUtilities.Hash(value.EnumValueJson);
            }
            value.LastUpdatedAtUtc = DateTime.UtcNow;
            value.LastUpdatedBy = updatedBy.Id;

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
            value.LastUpdatedBy = updatedBy.Id;

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
                value.LastUpdatedBy = updatedBy.Id;
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
}
