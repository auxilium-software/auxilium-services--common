using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework;
using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels;
using AuxiliumSoftware.AuxiliumServices.Common.Enumerators;
using AuxiliumSoftware.AuxiliumServices.Common.Utilities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace AuxiliumSoftware.AuxiliumServices.Common.Services.Implementations
{
    public class CaseAdditionalPropertyGroupService : ICaseAdditionalPropertyGroupService
    {
        private readonly AuxiliumDbContext _db;
        private readonly ILogger<CaseAdditionalPropertyGroupService> _logger;

        public CaseAdditionalPropertyGroupService(
            AuxiliumDbContext db,
            ILogger<CaseAdditionalPropertyGroupService> logger
        )
        {
            this._db = db;
            this._logger = logger;
        }

        #region ========================= READ OPERATIONS =========================
        public async Task<List<CaseAdditionalPropertyGroupEntityModel>> GetGroupsAsync(CancellationToken ct = default)
        {
            try
            {
                return await _db.WithinTenancy_CaseAdditionalProperty_Groups
                    .Include(g => g.Translations)
                    .OrderBy(g => g.SortOrder)
                    .ThenBy(g => g.CanonicalName)
                    .ToListAsync(ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get case additional property groups");
                throw;
            }
        }

        public async Task<CaseAdditionalPropertyGroupEntityModel?> GetGroupAsync(Guid groupId, CancellationToken ct = default)
        {
            try
            {
                return await _db.WithinTenancy_CaseAdditionalProperty_Groups
                    .Include(g => g.Translations)
                    .FirstOrDefaultAsync(g => g.Id == groupId, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get case additional property group {GroupId}", groupId);
                throw;
            }
        }
        #endregion
        #region ========================= GROUP OPERATIONS =========================
        public async Task<CaseAdditionalPropertyGroupEntityModel> CreateGroupAsync(
            Guid actorUserId,
            string canonicalName,
            IReadOnlyDictionary<string, string>? translations = null,
            CancellationToken ct = default
        )
        {
            try
            {
                if (await _db.WithinTenancy_CaseAdditionalProperty_Groups.AnyAsync(g => g.CanonicalName == canonicalName, ct))
                {
                    throw new InvalidOperationException($"A case additional property group named '{canonicalName}' already exists");
                }

                var nextSortOrder = (await _db.WithinTenancy_CaseAdditionalProperty_Groups
                    .MaxAsync(g => (int?)g.SortOrder, ct) ?? -1) + 1;

                var now = DateTime.UtcNow;

                var group = new CaseAdditionalPropertyGroupEntityModel
                {
                    Id = UUIDUtilities.GenerateV5(DatabaseObjectTypeEnum.WithinTenancy_Case_AdditionalProperty_Group),
                    CanonicalName = canonicalName,
                    SortOrder = nextSortOrder,
                    CreatedByUserId = actorUserId,
                    CreatedAtUtc = now,
                    Translations = new List<CaseAdditionalPropertyGroupTranslationEntityModel>(),
                };

                if (translations != null)
                {
                    foreach (var (languageCode, translation) in translations)
                    {
                        group.Translations.Add(new CaseAdditionalPropertyGroupTranslationEntityModel
                        {
                            Id = UUIDUtilities.GenerateV5(DatabaseObjectTypeEnum.WithinTenancy_Case_AdditionalProperty_Group_Translation),
                            CaseAdditionalPropertyGroupId = group.Id,
                            LanguageCode = languageCode.Trim(),
                            Translation = translation.Trim(),
                            CreatedByUserId = actorUserId,
                            CreatedAtUtc = now,
                        });
                    }
                }

                _db.WithinTenancy_CaseAdditionalProperty_Groups.Add(group);
                await _db.SaveChangesAsync(ct);

                _logger.LogInformation("Created case additional property group {GroupId} ({CanonicalName})", group.Id, canonicalName);

                return group;
            }
            catch (Exception ex) when (ex is not ArgumentException and not InvalidOperationException)
            {
                _logger.LogError(ex, "Failed to create case additional property group {CanonicalName}", canonicalName);
                throw;
            }
        }

        public async Task RenameGroupAsync(Guid actorUserId, Guid groupId, string newCanonicalName, CancellationToken ct = default)
        {
            try
            {
                var group = await _db.WithinTenancy_CaseAdditionalProperty_Groups
                    .FirstOrDefaultAsync(g => g.Id == groupId, ct)
                    ?? throw new KeyNotFoundException($"Case additional property group {groupId} not found");

                if (group.CanonicalName == newCanonicalName) return;

                if (await _db.WithinTenancy_CaseAdditionalProperty_Groups.AnyAsync(g => g.Id != groupId && g.CanonicalName == newCanonicalName, ct))
                    throw new InvalidOperationException($"A case additional property group named '{newCanonicalName}' already exists");

                var previousName = group.CanonicalName;

                group.CanonicalName = newCanonicalName;
                group.LastUpdatedAtUtc = DateTime.UtcNow;
                group.LastUpdatedByUserId = actorUserId;

                await _db.SaveChangesAsync(ct);

                _logger.LogInformation("Renamed case additional property group {GroupId} from {PreviousName} to {CanonicalName}",
                    groupId, previousName, newCanonicalName);
            }
            catch (Exception ex) when (ex is not KeyNotFoundException and not ArgumentException and not InvalidOperationException)
            {
                _logger.LogError(ex, "Failed to rename case additional property group {GroupId}", groupId);
                throw;
            }
        }

        public async Task ReorderGroupsAsync(Guid actorUserId, IReadOnlyList<Guid> orderedGroupIds, CancellationToken ct = default)
        {
            try
            {
                if (orderedGroupIds.Distinct().Count() != orderedGroupIds.Count)
                    throw new ArgumentException("The group ordering contains duplicate ids", nameof(orderedGroupIds));

                var groups = await _db.WithinTenancy_CaseAdditionalProperty_Groups.ToListAsync(ct);

                if (groups.Count != orderedGroupIds.Count || groups.Any(g => !orderedGroupIds.Contains(g.Id)))
                    throw new ArgumentException("The group ordering must contain every existing group exactly once", nameof(orderedGroupIds));

                var now = DateTime.UtcNow;
                var groupsById = groups.ToDictionary(g => g.Id);

                for (var i = 0; i < orderedGroupIds.Count; i++)
                {
                    var group = groupsById[orderedGroupIds[i]];
                    if (group.SortOrder == i) continue;

                    group.SortOrder = i;
                    group.LastUpdatedAtUtc = now;
                    group.LastUpdatedByUserId = actorUserId;
                }

                await _db.SaveChangesAsync(ct);

                _logger.LogInformation("Reordered {Count} case additional property groups", groups.Count);
            }
            catch (Exception ex) when (ex is not ArgumentException)
            {
                _logger.LogError(ex, "Failed to reorder case additional property groups");
                throw;
            }
        }

        public async Task<int> DeleteGroupAsync(Guid actorUserId, Guid groupId, CancellationToken ct = default)
        {
            try
            {
                var group = await _db.WithinTenancy_CaseAdditionalProperty_Groups
                    .Include(g => g.Translations)
                    .FirstOrDefaultAsync(g => g.Id == groupId, ct)
                    ?? throw new KeyNotFoundException($"Case additional property group {groupId} not found");

                var now = DateTime.UtcNow;
                var properties = await _db.WithinTenancy_CaseAdditionalProperties
                    .Where(p => p.GroupId == groupId)
                    .ToListAsync(ct);

                foreach (var property in properties)
                {
                    property.GroupId = null;
                    property.LastUpdatedAtUtc = now;
                    property.LastUpdatedByUserId = actorUserId;
                }

                _db.WithinTenancy_CaseAdditionalProperty_Groups.Remove(group);
                await _db.SaveChangesAsync(ct);

                _logger.LogInformation("Deleted case additional property group {GroupId} ({CanonicalName}), ungrouping {Count} properties",
                    groupId, group.CanonicalName, properties.Count);

                return properties.Count;
            }
            catch (Exception ex) when (ex is not KeyNotFoundException)
            {
                _logger.LogError(ex, "Failed to delete case additional property group {GroupId}", groupId);
                throw;
            }
        }
        #endregion
        #region ========================= TRANSLATION OPERATIONS =========================
        public async Task SetTranslationAsync(Guid actorUserId, Guid groupId, string languageCode, string translation, CancellationToken ct = default)
        {
            try
            {
                var group = await _db.WithinTenancy_CaseAdditionalProperty_Groups
                    .FirstOrDefaultAsync(g => g.Id == groupId, ct)
                    ?? throw new KeyNotFoundException($"Case additional property group {groupId} not found");

                var now = DateTime.UtcNow;

                var existing = await _db.WithinTenancy_CaseAdditionalProperty_Group_Translations
                    .FirstOrDefaultAsync(t => t.CaseAdditionalPropertyGroupId == groupId && t.LanguageCode == languageCode, ct);

                if (existing == null)
                {
                    _db.WithinTenancy_CaseAdditionalProperty_Group_Translations.Add(new CaseAdditionalPropertyGroupTranslationEntityModel
                    {
                        Id = UUIDUtilities.GenerateV5(DatabaseObjectTypeEnum.WithinTenancy_Case_AdditionalProperty_Group_Translation),
                        CaseAdditionalPropertyGroupId = groupId,
                        LanguageCode = languageCode,
                        Translation = translation,
                        CreatedByUserId = actorUserId,
                        CreatedAtUtc = now,
                    });
                }
                else
                {
                    if (existing.Translation == translation) return;

                    existing.Translation = translation;
                    existing.LastUpdatedAtUtc = now;
                    existing.LastUpdatedByUserId = actorUserId;
                }

                group.LastUpdatedAtUtc = now;
                group.LastUpdatedByUserId = actorUserId;

                await _db.SaveChangesAsync(ct);

                _logger.LogInformation("Set {LanguageCode} translation for case additional property group {GroupId}", languageCode, groupId);
            }
            catch (Exception ex) when (ex is not KeyNotFoundException and not ArgumentException)
            {
                _logger.LogError(ex, "Failed to set {LanguageCode} translation for case additional property group {GroupId}", languageCode, groupId);
                throw;
            }
        }

        public async Task RemoveTranslationAsync(Guid actorUserId, Guid groupId, string languageCode, CancellationToken ct = default)
        {
            try
            {
                var group = await _db.WithinTenancy_CaseAdditionalProperty_Groups
                    .FirstOrDefaultAsync(g => g.Id == groupId, ct)
                    ?? throw new KeyNotFoundException($"Case additional property group {groupId} not found");

                var translation = await _db.WithinTenancy_CaseAdditionalProperty_Group_Translations
                    .FirstOrDefaultAsync(t => t.CaseAdditionalPropertyGroupId == groupId && t.LanguageCode == languageCode, ct)
                    ?? throw new KeyNotFoundException($"No {languageCode} translation for case additional property group {groupId}");

                _db.WithinTenancy_CaseAdditionalProperty_Group_Translations.Remove(translation);

                group.LastUpdatedAtUtc = DateTime.UtcNow;
                group.LastUpdatedByUserId = actorUserId;

                await _db.SaveChangesAsync(ct);

                _logger.LogInformation("Removed {LanguageCode} translation from case additional property group {GroupId}", languageCode, groupId);
            }
            catch (Exception ex) when (ex is not KeyNotFoundException and not ArgumentException)
            {
                _logger.LogError(ex, "Failed to remove {LanguageCode} translation from case additional property group {GroupId}", languageCode, groupId);
                throw;
            }
        }
        #endregion
    }
}
