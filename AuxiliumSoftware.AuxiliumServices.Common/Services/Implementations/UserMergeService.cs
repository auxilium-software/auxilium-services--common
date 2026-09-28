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
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.Common.Services.Implementations
{
    public class UserMergeService : IUserMergeService
    {
        #region ========================= REFERENCE REGISTRY =========================
        public static readonly IReadOnlyList<IEntityMergeReference> References =
        [
            EntityMergeReference.Repoint(db => db.WithinTenancy_CalendarEvents,                             e => e.CreatedByUserId),
            EntityMergeReference.Repoint(db => db.WithinTenancy_CalendarEvents,                             e => e.LastUpdatedByUserId),
            EntityMergeReference.Repoint(db => db.WithinTenancy_CalendarEventInvites,                       e => e.CreatedByUserId),
            EntityMergeReference.Repoint(db => db.WithinTenancy_CalendarEventInvites,                       e => e.LastUpdatedByUserId),
            EntityMergeReference.Repoint(db => db.WithinTenancy_CalendarEventInvites,                       e => e.InvitedUserId, collidesOn: e => e.CalendarEventId),
            EntityMergeReference.Repoint(db => db.WithinTenancy_CalendarEventInvites,                       e => e.InvitedByUserId),
            EntityMergeReference.Repoint(db => db.WithinTenancy_Cases,                                      e => e.CreatedByUserId),
            EntityMergeReference.Repoint(db => db.WithinTenancy_Cases,                                      e => e.LastUpdatedByUserId),
            EntityMergeReference.Repoint(db => db.WithinTenancy_CaseAdditionalProperties,                   e => e.CreatedByUserId),
            EntityMergeReference.Repoint(db => db.WithinTenancy_CaseAdditionalProperties,                   e => e.LastUpdatedByUserId),
            EntityMergeReference.Repoint(db => db.WithinTenancy_CaseClients,                                e => e.CreatedByUserId),
            EntityMergeReference.Repoint(db => db.WithinTenancy_CaseClients,                                e => e.UserId, collidesOn: e => e.CaseId),
            EntityMergeReference.Repoint(db => db.WithinTenancy_CaseFiles,                                  e => e.CreatedByUserId),
            EntityMergeReference.Repoint(db => db.WithinTenancy_CaseFiles,                                  e => e.LastUpdatedByUserId),
            EntityMergeReference.Repoint(db => db.WithinTenancy_CaseMessages,                               e => e.CreatedByUserId),
            EntityMergeReference.Repoint(db => db.WithinTenancy_CaseMessages,                               e => e.LastUpdatedByUserId),
            EntityMergeReference.Repoint(db => db.WithinTenancy_CaseMessages,                               e => e.SenderUserId),
            EntityMergeReference.Repoint(db => db.WithinTenancy_CaseTimelineEntries,                        e => e.CreatedByUserId),
            EntityMergeReference.Repoint(db => db.WithinTenancy_CaseTimelineEntries,                        e => e.LastUpdatedByUserId),
            EntityMergeReference.Repoint(db => db.WithinTenancy_CaseTodos,                                  e => e.CreatedByUserId),
            EntityMergeReference.Repoint(db => db.WithinTenancy_CaseTodos,                                  e => e.LastUpdatedByUserId),
            EntityMergeReference.Repoint(db => db.WithinTenancy_CaseTodos,                                  e => e.AssignedToUserId),
            EntityMergeReference.Repoint(db => db.WithinTenancy_CaseTodos,                                  e => e.CompletedByUserId),
            EntityMergeReference.Repoint(db => db.WithinTenancy_CaseWorkers,                                e => e.CreatedByUserId),
            EntityMergeReference.Repoint(db => db.WithinTenancy_CaseWorkers,                                e => e.UserId, collidesOn: e => e.CaseId),
            EntityMergeReference.Repoint(db => db.WithinTenancy_DataEnumerator_Enumerators,                 e => e.CreatedByUserId),
            EntityMergeReference.Repoint(db => db.WithinTenancy_DataEnumerator_Enumerators,                 e => e.LastUpdatedByUserId),
            EntityMergeReference.Repoint(db => db.WithinTenancy_DataEnumerator_EnumeratorTranslations,      e => e.CreatedByUserId),
            EntityMergeReference.Repoint(db => db.WithinTenancy_DataEnumerator_EnumeratorTranslations,      e => e.LastUpdatedByUserId),
            EntityMergeReference.Repoint(db => db.WithinTenancy_DataEnumerator_EnumeratorValues,            e => e.CreatedByUserId),
            EntityMergeReference.Repoint(db => db.WithinTenancy_DataEnumerator_EnumeratorValues,            e => e.LastUpdatedByUserId),
            EntityMergeReference.Repoint(db => db.WithinTenancy_DataEnumerator_EnumeratorValueTranslations, e => e.CreatedByUserId),
            EntityMergeReference.Repoint(db => db.WithinTenancy_DataEnumerator_EnumeratorValueTranslations, e => e.LastUpdatedByUserId),
            EntityMergeReference.Repoint(db => db.WithinTenancy_Log_CaseMessageReadBys,                     e => e.CreatedByUserId, collidesOn: e => e.MessageId),
            EntityMergeReference.Repoint(db => db.WithinTenancy_Log_SystemBulletinEntryDismissals,          e => e.CreatedByUserId, collidesOn: e => e.SystemBulletinId),
            EntityMergeReference.Repoint(db => db.WithinTenancy_Log_SystemBulletinEntryViews,               e => e.CreatedByUserId),
            EntityMergeReference.Repoint(db => db.WithinTenancy_System_Bulletins,                           e => e.CreatedByUserId),
            EntityMergeReference.Repoint(db => db.WithinTenancy_System_Bulletins,                           e => e.SpecificUserId),
            EntityMergeReference.Repoint(db => db.WithinTenancy_System_Waf_UserBlacklist,                   e => e.UserId),
            EntityMergeReference.Repoint(db => db.WithinTenancy_Users,                                      e => e.CreatedByUserId),
            EntityMergeReference.Repoint(db => db.WithinTenancy_Users,                                      e => e.LastUpdatedByUserId),
            EntityMergeReference.Repoint(db => db.WithinTenancy_Users,                                      e => e.MergedIntoUserId),
            EntityMergeReference.Repoint(db => db.WithinTenancy_UserAdditionalProperties,                   e => e.CreatedByUserId),
            EntityMergeReference.Repoint(db => db.WithinTenancy_UserAdditionalProperties,                   e => e.LastUpdatedByUserId),
            EntityMergeReference.Repoint(db => db.WithinTenancy_UserAdditionalProperties,                   e => e.UserId),
            EntityMergeReference.Repoint(db => db.WithinTenancy_UserFiles,                                  e => e.CreatedByUserId),
            EntityMergeReference.Repoint(db => db.WithinTenancy_UserFiles,                                  e => e.LastUpdatedByUserId),
            EntityMergeReference.Repoint(db => db.WithinTenancy_UserFiles,                                  e => e.UserId),
            EntityMergeReference.Repoint(db => db.WithinTenancy_UserPasswordSetTokens,                      e => e.CreatedByUserId),
            EntityMergeReference.Repoint(db => db.WithinTenancy_UserWemwbsAssessments,                      e => e.CreatedByUserId),
            EntityMergeReference.Repoint(db => db.WithinTenancy_UserWemwbsAssessments,                      e => e.UserId),
            EntityMergeReference.Repoint(db => db.WithinTenancy_System_Waf_UserWhitelist,                   e => e.UserId),

            EntityMergeReference.Preserve(db => db.WithinTenancy_Cases,                                     e => e.MergedByUserId),
            EntityMergeReference.Preserve(db => db.WithinTenancy_Log_CaseModificationEvents,                e => e.CreatedByUserId),
            EntityMergeReference.Preserve(db => db.WithinTenancy_Log_CaseMergeEvents,                       e => e.CreatedByUserId),
            EntityMergeReference.Preserve(db => db.WithinTenancy_Log_UserMergeEvents,                       e => e.CreatedByUserId),
            EntityMergeReference.Preserve(db => db.WithinTenancy_Log_UserMergeEvents,                       e => e.SurvivorUserId),
            EntityMergeReference.Preserve(db => db.WithinTenancy_Log_UserMergeEvents,                       e => e.TombstoneUserId),
            EntityMergeReference.Preserve(db => db.WithinTenancy_Log_LoginAttempts,                         e => e.TargetUserId),
            EntityMergeReference.Preserve(db => db.WithinTenancy_Log_UserModificationEvents,                e => e.CreatedByUserId),
            EntityMergeReference.Preserve(db => db.WithinTenancy_Log_UserModificationEvents,                e => e.UserId),
            EntityMergeReference.Preserve(db => db.WithinTenancy_System_Settings,                           e => e.CreatedByUserId),
            EntityMergeReference.Preserve(db => db.WithinTenancy_System_Waf_IpBlacklist,                    e => e.CreatedByUserId),
            EntityMergeReference.Preserve(db => db.WithinTenancy_System_Waf_IpBlacklist,                    e => e.UnblacklistedBy),
            EntityMergeReference.Preserve(db => db.WithinTenancy_System_Waf_IpWhitelist,                    e => e.CreatedByUserId),
            EntityMergeReference.Preserve(db => db.WithinTenancy_System_Waf_UserBlacklist,                  e => e.CreatedByUserId),
            EntityMergeReference.Preserve(db => db.WithinTenancy_System_Waf_UserBlacklist,                  e => e.UnblacklistedBy),
            EntityMergeReference.Preserve(db => db.WithinTenancy_System_Waf_UserWhitelist,                  e => e.CreatedByUserId),
            EntityMergeReference.Preserve(db => db.WithinTenancy_Users,                                     e => e.MergedByUserId),
            EntityMergeReference.Preserve(db => db.WithinTenancy_System_Waf_IpWhitelist,                    e => e.UnwhitelistedBy),
            EntityMergeReference.Preserve(db => db.WithinTenancy_System_Waf_UserWhitelist,                  e => e.UnwhitelistedBy),

            EntityMergeReference.Discard(db => db.WithinTenancy_UserPasswordSetTokens,                      e => e.UserId),
            EntityMergeReference.Discard(db => db.WithinTenancy_UserRefreshTokens,                          e => e.CreatedByUserId),
            EntityMergeReference.Discard(db => db.WithinTenancy_UserTotpRecoveryCodes,                      e => e.CreatedByUserId),
        ];

        public static void VerifyReferenceCoverage(IModel model) =>
            DataMergeReferenceUtilities.VerifyCoverage(model, typeof(UserEntityModel), References, "user_id", "_by");

        private static int _coverageVerified;
        #endregion





        private static readonly string[] SnapshotExclusions =
        [
            nameof(UserEntityModel.PasswordHash),
            nameof(UserEntityModel.TotpSecret),
            nameof(UserEntityModel.ConcurrencyStamp),
        ];

        private static readonly string EmailAddressFieldKey =
            char.ToLowerInvariant(nameof(UserEntityModel.EmailAddress)[0]) + nameof(UserEntityModel.EmailAddress)[1..];

        private static readonly JsonSerializerOptions RecordJsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
        };

        private static readonly DataMergeFieldResolver<UserEntityModel> FieldResolver = new();

        private readonly AuxiliumDbContext _db;
        private readonly IUserDocumentService _userDocService;
        private readonly ILogger<UserMergeService> _logger;

        public UserMergeService(
            AuxiliumDbContext db,
            IUserDocumentService userDocService,
            ILogger<UserMergeService> logger
        )
        {
            this._db = db;
            this._userDocService = userDocService;
            this._logger = logger;
        }





        #region ========================= PREVIEW =========================
        public async Task<DataMergePreviewDTO> PreviewAsync(
            Guid survivorUserId,
            Guid duplicateUserId,
            Guid actorUserId,
            CancellationToken ct = default
        )
        {
            EnsureCoverageVerified();

            (UserEntityModel survivor, UserEntityModel duplicate) = await LoadPairAsync(survivorUserId, duplicateUserId, actorUserId, track: false, ct);

            List<DataMergeReferenceSummaryDTO> references = new();
            foreach (IEntityMergeReference reference in References)
            {
                DataMergeReferenceCounts counts = await reference.CountAsync(_db, duplicate.Id, survivor.Id, ct);
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
                Fields = FieldResolver.BuildPreviewFields(survivor, duplicate),
                References = references,
            };
        }
        #endregion





        #region ========================= MERGE =========================
        public async Task<DataMergeResultDTO> MergeAsync(
            Guid survivorUserId,
            Guid duplicateUserId,
            string fingerprint,
            IReadOnlyDictionary<string, DataMergeSourceEnum>? fieldChoices,
            string justification,
            Guid actorUserId,
            CancellationToken ct = default
        )
        {
            EnsureCoverageVerified();

            Dictionary<string, DataMergeSourceEnum> choices = FieldResolver.ParseChoices(fieldChoices);

            IExecutionStrategy strategy = _db.Database.CreateExecutionStrategy();

            try
            {
                return await strategy.ExecuteAsync(async () =>
                {
                    _db.ChangeTracker.Clear();
                    await using IDbContextTransaction transaction = await _db.Database.BeginTransactionAsync(ct);

                    (UserEntityModel survivor, UserEntityModel duplicate) = await LoadPairAsync(survivorUserId, duplicateUserId, actorUserId, track: true, ct);

                    if (!string.Equals(ComputeFingerprint(survivor, duplicate), fingerprint?.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        throw new DataMergeRejectedException(
                            DataMergeRejectionReasonEnum.Stale,
                            "One of the users has changed since this merge was previewed. Please review the merge again."
                        );
                    }

                    DateTime now = DateTime.UtcNow;
                    Dictionary<string, string?> survivorBefore = DataMergeFieldResolver<UserEntityModel>.SnapshotScalars(_db.Model, survivor, SnapshotExclusions);
                    Dictionary<string, string?> duplicateSnapshot = DataMergeFieldResolver<UserEntityModel>.SnapshotScalars(_db.Model, duplicate, SnapshotExclusions);


                    bool survivorVerifiedBefore = survivor.HasEmailAddressBeenVerified;
                    DataMergeFieldResolver<UserEntityModel>.Outcome outcome = FieldResolver.Apply(survivor, duplicate, choices);
                    List<(string PropertyName, string? Previous, string? Next)> fieldChanges = outcome.Changes;

                    if (outcome.Sources.TryGetValue(EmailAddressFieldKey, out DataMergeSourceEnum emailSource)
                        && emailSource == DataMergeSourceEnum.Duplicate
                        && !string.Equals(survivorBefore[nameof(UserEntityModel.EmailAddress)], duplicateSnapshot[nameof(UserEntityModel.EmailAddress)], StringComparison.Ordinal))
                    {
                        survivor.HasEmailAddressBeenVerified = duplicate.HasEmailAddressBeenVerified;
                        if (survivor.HasEmailAddressBeenVerified != survivorVerifiedBefore)
                        {
                            fieldChanges.Add((
                                nameof(UserEntityModel.HasEmailAddressBeenVerified),
                                DataMergeFieldResolver<UserEntityModel>.FormatValue(survivorVerifiedBefore),
                                DataMergeFieldResolver<UserEntityModel>.FormatValue(survivor.HasEmailAddressBeenVerified)
                            ));
                        }
                    }

                    await _db.WithinTenancy_Users
                        .Where(u => u.Id == duplicate.Id)
                        .ExecuteUpdateAsync(setters => setters.SetProperty(u => u.EmailAddress, (string?)null), ct);


                    List<DataMergeReferenceChange> referenceChanges = new();
                    foreach (IEntityMergeReference reference in References)
                    {
                        referenceChanges.Add(await reference.ApplyAsync(_db, duplicate.Id, survivor.Id, ct));
                    }


                    survivor.LastUpdatedAtUtc = now;
                    survivor.LastUpdatedByUserId = actorUserId;

                    duplicate.MergedIntoUserId = survivor.Id;
                    duplicate.MergedAtUtc = now;
                    duplicate.MergedByUserId = actorUserId;
                    duplicate.EmailAddress = null;
                    duplicate.HasEmailAddressBeenVerified = false;
                    duplicate.PasswordHash = null;
                    duplicate.MustChangePassword = false;
                    duplicate.TotpSecret = null;
                    duplicate.TotpEnabled = false;
                    duplicate.TotpEnabledAtUtc = null;
                    duplicate.AllowLogin = false;
                    duplicate.LastUpdatedAtUtc = now;
                    duplicate.LastUpdatedByUserId = actorUserId;


                    foreach ((string propertyName, string? previous, string? next) in fieldChanges)
                    {
                        await _userDocService.WriteToAuditLog(
                            actorUserId: actorUserId,
                            targetUserId: survivor.Id,
                            actionType: AuditLogActionTypeEnum.Modification,
                            entityType: UserEntityTypeEnum.User,
                            entityId: survivor.Id,
                            propertyName: propertyName,
                            oldValue: previous,
                            newValue: next,
                            ct: ct
                        );
                    }
                    await _userDocService.WriteToAuditLog(
                        actorUserId: actorUserId,
                        targetUserId: survivor.Id,
                        actionType: AuditLogActionTypeEnum.Merge,
                        entityType: UserEntityTypeEnum.User,
                        entityId: duplicate.Id,
                        ct: ct
                    );
                    await _userDocService.WriteToAuditLog(
                        actorUserId: actorUserId,
                        targetUserId: duplicate.Id,
                        actionType: AuditLogActionTypeEnum.Merge,
                        entityType: UserEntityTypeEnum.User,
                        entityId: survivor.Id,
                        ct: ct
                    );

                    Guid mergeEventId = UUIDUtilities.GenerateV5(DatabaseObjectTypeEnum.WithinTenancy_Log_UserMerge_EventEntry);

                    _db.WithinTenancy_Log_UserMergeEvents.Add(new LogUserMergeEventEntityModel
                    {
                        Id = mergeEventId,
                        CreatedAtUtc = now,
                        CreatedByUserId = actorUserId,
                        SurvivorUserId = survivor.Id,
                        TombstoneUserId = duplicate.Id,
                        Justification = justification,
                        FieldResolutionsJson = JsonSerializer.Serialize(outcome.Resolutions, RecordJsonOptions),
                        SurvivorPreviousValuesJson = JsonSerializer.Serialize(survivorBefore, RecordJsonOptions),
                        TombstoneSnapshotJson = JsonSerializer.Serialize(duplicateSnapshot, RecordJsonOptions),
                    });
                    _db.WithinTenancy_Log_UserMergeEventRowChanges.AddRange(BuildRowChanges(mergeEventId, referenceChanges, now));

                    await _db.SaveChangesAsync(ct);
                    await transaction.CommitAsync(ct);

                    long rowsMoved = referenceChanges.Where(c => c.Behaviour == DataMergeBehaviourEnum.Repoint).Sum(c => c.RowsAffected);
                    long rowsDiscarded = referenceChanges.Sum(c => (long)c.DiscardedRows.Count);

                    _logger.LogInformation(
                        "User {DuplicateUserId} merged into {SurvivorUserId} by {ActorUserId}: {RowsMoved} rows moved, {RowsDiscarded} discarded (merge event {MergeEventId})",
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
                    "One of the users was changed while the merge was running (for example by signing in). Nothing was merged; please review it again."
                );
            }
        }
        #endregion





        #region ========================= HELPERS =========================
        private static IEnumerable<LogUserMergeEventRowChangeEntityModel> BuildRowChanges(
            Guid mergeEventId,
            IEnumerable<DataMergeReferenceChange> referenceChanges,
            DateTime now
        )
        {
            foreach (DataMergeReferenceChange change in referenceChanges)
            {
                foreach (Guid rowId in change.MovedRowIds)
                {
                    yield return new LogUserMergeEventRowChangeEntityModel
                    {
                        Id = UUIDUtilities.GenerateV5(DatabaseObjectTypeEnum.WithinTenancy_Log_UserMerge_RowChange_EventEntry),
                        CreatedAtUtc = now,
                        UserMergeEventId = mergeEventId,
                        EntityName = change.Entity,
                        PropertyName = change.Property,
                        RowId = rowId,
                        Action = DataMergeRowActionEnum.Moved,
                    };
                }

                foreach (DataMergeDiscardedRow discarded in change.DiscardedRows)
                {
                    yield return new LogUserMergeEventRowChangeEntityModel
                    {
                        Id = UUIDUtilities.GenerateV5(DatabaseObjectTypeEnum.WithinTenancy_Log_UserMerge_RowChange_EventEntry),
                        CreatedAtUtc = now,
                        UserMergeEventId = mergeEventId,
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

        private async Task<(UserEntityModel Survivor, UserEntityModel Duplicate)> LoadPairAsync(
            Guid survivorUserId,
            Guid duplicateUserId,
            Guid actorUserId,
            bool track,
            CancellationToken ct
        )
        {
            if (survivorUserId == duplicateUserId)
            {
                throw new DataMergeRejectedException(DataMergeRejectionReasonEnum.Invalid, "A user cannot be merged into themselves.");
            }

            if (duplicateUserId == actorUserId)
            {
                throw new DataMergeRejectedException(
                    DataMergeRejectionReasonEnum.Invalid,
                    "You cannot merge away your own account. Keep your account and merge the other one into it, or ask another administrator."
                );
            }

            IQueryable<UserEntityModel> query = _db.WithinTenancy_Users;
            if (!track)
            {
                query = query.AsNoTracking();
            }

            List<UserEntityModel> pair = await query
                .Where(u => u.Id == survivorUserId || u.Id == duplicateUserId)
                .ToListAsync(ct);

            UserEntityModel survivor = pair.FirstOrDefault(u => u.Id == survivorUserId)
                ?? throw new DataMergeRejectedException(DataMergeRejectionReasonEnum.NotFound, "The user to keep does not exist.");
            UserEntityModel duplicate = pair.FirstOrDefault(u => u.Id == duplicateUserId)
                ?? throw new DataMergeRejectedException(DataMergeRejectionReasonEnum.NotFound, "The duplicate user does not exist.");

            if (survivor.MergedIntoUserId is not null)
            {
                throw new DataMergeRejectedException(
                    DataMergeRejectionReasonEnum.Invalid,
                    "The user to keep has themselves already been merged into another user."
                );
            }
            if (duplicate.MergedIntoUserId is not null)
            {
                throw new DataMergeRejectedException(
                    DataMergeRejectionReasonEnum.Invalid,
                    "The duplicate user has already been merged into another user."
                );
            }

            if (survivor.DeletionRequested || duplicate.DeletionRequested)
            {
                throw new DataMergeRejectedException(
                    DataMergeRejectionReasonEnum.Invalid,
                    "One of these users has asked for their account to be deleted. Deal with that request before merging."
                );
            }

            return (survivor, duplicate);
        }

        private static string ComputeFingerprint(UserEntityModel survivor, UserEntityModel duplicate) =>
            FieldResolver.ComputeFingerprint(survivor, duplicate, user => new string?[]
            {
                user.Id.ToString(),
                user.ConcurrencyStamp.ToString(),
                DataMergeFieldResolver<UserEntityModel>.FormatValue(user.MergedIntoUserId),
            });
        #endregion
    }
}
