using AuxiliumSoftware.AuxiliumServices.Common.Attributes;
using AuxiliumSoftware.AuxiliumServices.Common.DataTransferObjects;
using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework;
using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels;
using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators;
using AuxiliumSoftware.AuxiliumServices.Common.Enumerators;
using AuxiliumSoftware.AuxiliumServices.Common.Exceptions;
using AuxiliumSoftware.AuxiliumServices.Common.Factories;
using AuxiliumSoftware.AuxiliumServices.Common.Interfaces;
using AuxiliumSoftware.AuxiliumServices.Common.Records;
using AuxiliumSoftware.AuxiliumServices.Common.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.Common.Services.Implementations
{
    public class CaseMergeService : ICaseMergeService
    {
        #region ========================= REFERENCE REGISTRY =========================
        public static readonly IReadOnlyList<IEntityMergeReference> References =
        [
            EntityMergeReference.Repoint(db => db.WithinTenancy_CalendarEvents,             e => e.CaseId),
            EntityMergeReference.Repoint(db => db.WithinTenancy_CaseAdditionalProperties,   e => e.CaseId),
            EntityMergeReference.Repoint(db => db.WithinTenancy_CaseClients,                e => e.CaseId, collidesOn: e => e.UserId),
            EntityMergeReference.Repoint(db => db.WithinTenancy_CaseFiles,                  e => e.CaseId),
            EntityMergeReference.Repoint(db => db.WithinTenancy_CaseMessages,               e => e.CaseId),
            EntityMergeReference.Repoint(db => db.WithinTenancy_CaseTimelineEntries,        e => e.CaseId),
            EntityMergeReference.Repoint(db => db.WithinTenancy_CaseTodos,                  e => e.CaseId),
            EntityMergeReference.Repoint(db => db.WithinTenancy_CaseWorkers,                e => e.CaseId, collidesOn: e => e.UserId),

            EntityMergeReference.Preserve(db => db.WithinTenancy_Log_CaseModificationEvents, e => e.CaseId),
            EntityMergeReference.Preserve(db => db.WithinTenancy_Log_CaseMergeEvents,        e => e.SurvivorCaseId),
            EntityMergeReference.Preserve(db => db.WithinTenancy_Log_CaseMergeEvents,        e => e.TombstoneCaseId),

            EntityMergeReference.Repoint(db => db.WithinTenancy_Cases,                      e => e.MergedIntoCaseId),
        ];

        public static void VerifyReferenceCoverage(IModel model) =>
            DataMergeReferenceUtilities.VerifyCoverage(model, typeof(CaseEntityModel), References, "case_id");

        private static int _coverageVerified;
        #endregion





        private const string CombineSeparator = "\n\n";

        private static readonly JsonSerializerOptions RecordJsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
        };

        private static readonly IReadOnlyList<ResolvableField> ResolvableFields = BuildResolvableFields();

        private readonly AuxiliumDbContext _db;
        private readonly ICaseDocumentService _caseDocService;
        private readonly ILogger<CaseMergeService> _logger;

        public CaseMergeService(
            AuxiliumDbContext db,
            ICaseDocumentService caseDocService,
            ILogger<CaseMergeService> logger
        )
        {
            this._db = db;
            this._caseDocService = caseDocService;
            this._logger = logger;
        }





        #region ========================= PREVIEW =========================
        public async Task<DataMergePreviewDTO> PreviewAsync(Guid survivorCaseId, Guid duplicateCaseId, CancellationToken ct = default)
        {
            EnsureCoverageVerified();

            var (survivor, duplicate) = await LoadPairAsync(survivorCaseId, duplicateCaseId, track: false, ct);

            var references = new List<DataMergeReferenceSummaryDTO>();
            foreach (var reference in References)
            {
                var counts = await reference.CountAsync(_db, duplicate.Id, survivor.Id, ct);
                if (counts.Total == 0)
                {
                    continue;
                }

                references.Add(new DataMergeReferenceSummaryDTO
                {
                    Entity = reference.EntityType.Name.EndsWith("EntityModel", StringComparison.Ordinal)
                        ? reference.EntityType.Name[..^"EntityModel".Length]
                        : reference.EntityType.Name,
                    Property = reference.PropertyName,
                    Behaviour = reference.Behaviour,
                    RowsToMove = reference.Behaviour == DataMergeBehaviourEnum.Repoint ? counts.Total - counts.Colliding : 0,
                    RowsToDiscard = reference.Behaviour switch
                    {
                        DataMergeBehaviourEnum.Repoint => counts.Colliding,
                        DataMergeBehaviourEnum.Discard => counts.Total,
                        _ => 0,
                    },
                    RowsKeptOnDuplicate = reference.Behaviour == DataMergeBehaviourEnum.Preserve ? counts.Total : 0,
                });
            }

            return new DataMergePreviewDTO
            {
                SurvivorId = survivor.Id,
                DuplicateId = duplicate.Id,
                Fingerprint = ComputeFingerprint(survivor, duplicate),
                Fields = ResolvableFields.Select(field =>
                {
                    var survivorValue = field.Property.GetValue(survivor);
                    var duplicateValue = field.Property.GetValue(duplicate);

                    return new DataMergeFieldConflictDTO
                    {
                        Field = field.Key,
                        SurvivorValue = FormatValue(survivorValue),
                        DuplicateValue = FormatValue(duplicateValue),
                        Differs = !Equals(survivorValue, duplicateValue),
                        CanCombine = field.Attribute.AllowCombine,
                        SuggestedSource = SuggestSource(field, survivorValue, duplicateValue),
                    };
                }).ToList(),
                References = references,
            };
        }
        #endregion





        #region ========================= MERGE =========================
        public async Task<DataMergeResultDTO> MergeAsync(
            Guid survivorCaseId,
            Guid duplicateCaseId,
            string fingerprint,
            IReadOnlyDictionary<string, DataMergeSourceEnum>? fieldChoices,
            string justification,
            Guid actorUserId,
            CancellationToken ct = default
        )
        {
            EnsureCoverageVerified();

            Dictionary<string, DataMergeSourceEnum> choices = ParseChoices(fieldChoices);

            IExecutionStrategy strategy = _db.Database.CreateExecutionStrategy();

            try
            {
                return await strategy.ExecuteAsync(async () =>
                {
                    _db.ChangeTracker.Clear();
                    await using IDbContextTransaction transaction = await _db.Database.BeginTransactionAsync(ct);

                    (CaseEntityModel survivor, CaseEntityModel duplicate) = await LoadPairAsync(survivorCaseId, duplicateCaseId, track: true, ct);

                    if (!string.Equals(ComputeFingerprint(survivor, duplicate), fingerprint?.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        throw new DataMergeRejectedException(
                            DataMergeRejectionReasonEnum.Stale,
                            "One of the cases has changed since this merge was previewed. Please review the merge again."
                        );
                    }

                    DateTime now = DateTime.UtcNow;
                    Dictionary<string, string?> survivorBefore = SnapshotScalars(survivor);
                    Dictionary<string, string?> duplicateSnapshot = SnapshotScalars(duplicate);


                    List<object> resolutions = new();
                    List<(string PropertyName, string? Previous, string? Next)> fieldChanges = new();

                    foreach (var field in ResolvableFields)
                    {
                        var survivorValue = field.Property.GetValue(survivor);
                        var duplicateValue = field.Property.GetValue(duplicate);

                        DataMergeSourceEnum source = choices.TryGetValue(field.Key, out var chosen)
                            ? chosen
                            : SuggestSource(field, survivorValue, duplicateValue);

                        var resolved = source switch
                        {
                            DataMergeSourceEnum.Survivor => survivorValue,
                            DataMergeSourceEnum.Duplicate => duplicateValue,
                            DataMergeSourceEnum.Combined => Combine((string?)survivorValue, (string?)duplicateValue),
                            _ => throw new InvalidOperationException($"Unhandled merge source {source}."),
                        };

                        if (!Equals(survivorValue, resolved))
                        {
                            field.Property.SetValue(survivor, resolved);
                            fieldChanges.Add((field.Property.Name, FormatValue(survivorValue), FormatValue(resolved)));
                        }

                        resolutions.Add(new { field = field.Key, source, value = FormatValue(resolved) });
                    }


                    List<DataMergeReferenceChange> referenceChanges = new();
                    foreach (IEntityMergeReference reference in References)
                    {
                        referenceChanges.Add(await reference.ApplyAsync(_db, duplicate.Id, survivor.Id, ct));
                    }


                    survivor.LastUpdatedAtUtc = now;
                    survivor.LastUpdatedByUserId = actorUserId;

                    duplicate.MergedIntoCaseId = survivor.Id;
                    duplicate.MergedAtUtc = now;
                    duplicate.MergedByUserId = actorUserId;
                    duplicate.LastUpdatedAtUtc = now;
                    duplicate.LastUpdatedByUserId = actorUserId;


                    foreach ((string propertyName, string? previous, string? next) in fieldChanges)
                    {
                        await _caseDocService.WriteToAuditLog(
                            actorUserId: actorUserId,
                            caseId: survivor.Id,
                            actionType: AuditLogActionTypeEnum.Modification,
                            entityType: CaseEntityTypeEnum.Case,
                            entityId: survivor.Id,
                            propertyName: propertyName,
                            oldValue: previous,
                            newValue: next,
                            ct: ct
                        );
                    }
                    await _caseDocService.WriteToAuditLog(
                        actorUserId: actorUserId,
                        caseId: survivor.Id,
                        actionType: AuditLogActionTypeEnum.Merge,
                        entityType: CaseEntityTypeEnum.Case,
                        entityId: duplicate.Id,
                        ct: ct
                    );
                    await _caseDocService.WriteToAuditLog(
                        actorUserId: actorUserId,
                        caseId: duplicate.Id,
                        actionType: AuditLogActionTypeEnum.Merge,
                        entityType: CaseEntityTypeEnum.Case,
                        entityId: survivor.Id,
                        ct: ct
                    );

                    _db.WithinTenancy_CaseTimelineEntries.Add(new CaseTimelineEntryEntityModel
                    {
                        Id = UUIDUtilities.GenerateV5(DatabaseObjectTypeEnum.WithinTenancy_Case_TimelineEntry),
                        CaseId = survivor.Id,
                        EntryType = CaseTimelineEntryTypeEnum.Note_System,
                        OccurredAtUtc = now,
                        Title = "Case merged",
                        Description = $"The case \"{duplicate.Title}\" ({duplicate.Id}) was merged into this case.\n\nReason: {justification}",
                        CreatedAtUtc = now,
                        CreatedByUserId = actorUserId,
                    });

                    Guid mergeEventId = UUIDUtilities.GenerateV5(DatabaseObjectTypeEnum.WithinTenancy_Log_CaseMerge_EventEntry);

                    _db.WithinTenancy_Log_CaseMergeEvents.Add(new LogCaseMergeEventEntityModel
                    {
                        Id = mergeEventId,
                        CreatedAtUtc = now,
                        CreatedByUserId = actorUserId,
                        SurvivorCaseId = survivor.Id,
                        TombstoneCaseId = duplicate.Id,
                        Justification = justification,
                        FieldResolutionsJson = JsonSerializer.Serialize(resolutions, RecordJsonOptions),
                        SurvivorPreviousValuesJson = JsonSerializer.Serialize(survivorBefore, RecordJsonOptions),
                        TombstoneSnapshotJson = JsonSerializer.Serialize(duplicateSnapshot, RecordJsonOptions),
                    });
                    _db.WithinTenancy_Log_CaseMergeEventRowChanges.AddRange(BuildRowChanges(mergeEventId, referenceChanges, now));

                    await _db.SaveChangesAsync(ct);
                    await transaction.CommitAsync(ct);

                    long rowsMoved = referenceChanges.Where(c => c.Behaviour == DataMergeBehaviourEnum.Repoint).Sum(c => c.RowsAffected);
                    long rowsDiscarded = referenceChanges.Sum(c => (long)c.DiscardedRows.Count);

                    _logger.LogInformation(
                        "Case {DuplicateCaseId} merged into {SurvivorCaseId} by {ActorUserId}: {RowsMoved} rows moved, {RowsDiscarded} discarded (merge event {MergeEventId})",
                        duplicate.Id,
                        survivor.Id,
                        actorUserId,
                        rowsMoved,
                        rowsDiscarded,
                        mergeEventId
                    );

                    return new DataMergeResultDTO
                    {
                        MergeEventId = mergeEventId,
                        SurvivorId = survivor.Id,
                        DuplicateId = duplicate.Id,
                        RowsMoved = rowsMoved,
                        RowsDiscarded = rowsDiscarded,
                    };
                });
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new DataMergeRejectedException(
                    DataMergeRejectionReasonEnum.Stale,
                    "One of the cases was changed by someone else while the merge was running. Nothing was merged; please review it again."
                );
            }
        }
        #endregion





        #region ========================= HELPERS =========================
        private static IEnumerable<LogCaseMergeEventRowChangeEntityModel> BuildRowChanges(
            Guid mergeEventId,
            IEnumerable<DataMergeReferenceChange> referenceChanges,
            DateTime now
        )
        {
            foreach (DataMergeReferenceChange change in referenceChanges)
            {
                foreach (Guid rowId in change.MovedRowIds)
                {
                    yield return new LogCaseMergeEventRowChangeEntityModel
                    {
                        Id = UUIDUtilities.GenerateV5(DatabaseObjectTypeEnum.WithinTenancy_Log_CaseMerge_RowChange_EventEntry),
                        CreatedAtUtc = now,
                        CaseMergeEventId = mergeEventId,
                        EntityName = change.Entity,
                        PropertyName = change.Property,
                        RowId = rowId,
                        Action = DataMergeRowActionEnum.Moved,
                    };
                }

                foreach (DataMergeDiscardedRow discarded in change.DiscardedRows)
                {
                    yield return new LogCaseMergeEventRowChangeEntityModel
                    {
                        Id = UUIDUtilities.GenerateV5(DatabaseObjectTypeEnum.WithinTenancy_Log_CaseMerge_RowChange_EventEntry),
                        CreatedAtUtc = now,
                        CaseMergeEventId = mergeEventId,
                        EntityName = change.Entity,
                        PropertyName = change.Property,
                        RowId = discarded.Id,
                        Action = DataMergeRowActionEnum.Discarded,
                        SnapshotJson = discarded.SnapshotJson,
                    };
                }
            }
        }

        private void EnsureCoverageVerified()
        {
            if (Volatile.Read(ref _coverageVerified) == 1)
            {
                return;
            }

            VerifyReferenceCoverage(_db.Model);
            Volatile.Write(ref _coverageVerified, 1);
        }

        private async Task<(CaseEntityModel Survivor, CaseEntityModel Duplicate)> LoadPairAsync(
            Guid survivorCaseId,
            Guid duplicateCaseId,
            bool track,
            CancellationToken ct
        )
        {
            if (survivorCaseId == duplicateCaseId)
            {
                throw new DataMergeRejectedException(DataMergeRejectionReasonEnum.Invalid, "A case cannot be merged into itself.");
            }

            IQueryable<CaseEntityModel> query = _db.WithinTenancy_Cases;
            if (!track)
            {
                query = query.AsNoTracking();
            }

            var pair = await query
                .Where(c => c.Id == survivorCaseId || c.Id == duplicateCaseId)
                .ToListAsync(ct);

            CaseEntityModel? survivor = pair.FirstOrDefault(c => c.Id == survivorCaseId)
                ?? throw new DataMergeRejectedException(DataMergeRejectionReasonEnum.NotFound, "The case to keep does not exist.");
            CaseEntityModel? duplicate = pair.FirstOrDefault(c => c.Id == duplicateCaseId)
                ?? throw new DataMergeRejectedException(DataMergeRejectionReasonEnum.NotFound, "The duplicate case does not exist.");

            if (survivor.MergedIntoCaseId is not null)
            {
                throw new DataMergeRejectedException(
                    DataMergeRejectionReasonEnum.Invalid,
                    "The case to keep has itself already been merged into another case."
                );
            }
            if (duplicate.MergedIntoCaseId is not null)
            {
                throw new DataMergeRejectedException(
                    DataMergeRejectionReasonEnum.Invalid,
                    "The duplicate case has already been merged into another case."
                );
            }

            return (survivor, duplicate);
        }

        private static Dictionary<string, DataMergeSourceEnum> ParseChoices(IReadOnlyDictionary<string, DataMergeSourceEnum>? fieldChoices)
        {
            Dictionary<string, DataMergeSourceEnum> result = new(StringComparer.OrdinalIgnoreCase);
            if (fieldChoices is null)
            {
                return result;
            }

            foreach ((string key, DataMergeSourceEnum source) in fieldChoices)
            {
                ResolvableField? field = ResolvableFields.FirstOrDefault(f => string.Equals(f.Key, key, StringComparison.OrdinalIgnoreCase))
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

        private static DataMergeSourceEnum SuggestSource(ResolvableField field, object? survivorValue, object? duplicateValue)
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

        private static string? Combine(string? survivorValue, string? duplicateValue)
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

        private static string ComputeFingerprint(CaseEntityModel survivor, CaseEntityModel duplicate)
        {
            StringBuilder builder = new();

            void Append(string? value) =>
                builder.Append(value?.Length ?? -1).Append(':').Append(value).Append(';');

            foreach (CaseEntityModel? caseEntity in new[] { survivor, duplicate })
            {
                Append(caseEntity.Id.ToString());
                Append(caseEntity.ConcurrencyStamp.ToString());
                Append(FormatValue(caseEntity.MergedIntoCaseId));

                foreach (ResolvableField? field in ResolvableFields)
                {
                    Append(FormatValue(field.Property.GetValue(caseEntity)));
                }
            }

            return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(builder.ToString())));
        }

        private Dictionary<string, string?> SnapshotScalars(CaseEntityModel caseEntity)
        {
            return _db.Model.FindEntityType(typeof(CaseEntityModel))!
                .GetProperties()
                .Where(p => p.PropertyInfo is not null)
                .ToDictionary(
                    p => p.Name,
                    p => FormatValue(
                        p.PropertyInfo!.GetValue(caseEntity)
                    )
                );
        }

        private static bool IsEmpty(object? value) =>
            value is null
            || (
                value is string s
                && string.IsNullOrWhiteSpace(s)
            );

        private static string? FormatValue(object? value) => value switch
        {
            null                        => null,
            Enum enumValue              => enumValue.GetType().GetField(enumValue.ToString())?.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? enumValue.ToString(),
            DateTime dateTime           => dateTime.ToString("O", CultureInfo.InvariantCulture),
            IFormattable formattable    => formattable.ToString(null, CultureInfo.InvariantCulture),
            _                           => value.ToString(),
        };

        private static IReadOnlyList<ResolvableField> BuildResolvableFields()
        {
            List<ResolvableField>? fields = typeof(CaseEntityModel)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(p => (Property: p, Attribute: p.GetCustomAttribute<DataMergeResolvableAttribute>()))
                .Where(x => x.Attribute is not null)
                .Select(x => new ResolvableField(
                    char.ToLowerInvariant(x.Property.Name[0]) + x.Property.Name[1..],
                    x.Property,
                    x.Attribute!))
                .ToList();

            foreach (ResolvableField? field in fields.Where(f => f.Attribute.AllowCombine && f.Property.PropertyType != typeof(string)))
            {
                throw new InvalidOperationException($"CaseEntityModel.{field.Property.Name} allows combining on merge, but only string properties can be combined.");
            }

            return fields;
        }

        private sealed record ResolvableField(string Key, PropertyInfo Property, DataMergeResolvableAttribute Attribute);
        #endregion
    }
}
