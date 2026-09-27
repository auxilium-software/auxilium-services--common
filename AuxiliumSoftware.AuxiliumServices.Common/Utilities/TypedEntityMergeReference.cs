using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework;
using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Abstractions;
using AuxiliumSoftware.AuxiliumServices.Common.Enumerators;
using AuxiliumSoftware.AuxiliumServices.Common.Interfaces;
using AuxiliumSoftware.AuxiliumServices.Common.Records;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.Common.Utilities
{
    internal sealed class TypedEntityMergeReference<TEntity, TKey> : IEntityMergeReference
        where TEntity : TenantScopedEntityModelBase
    {
        private static readonly JsonSerializerOptions SnapshotJsonOptions = new()
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
        };

        private readonly Func<AuxiliumDbContext, DbSet<TEntity>> _set;
        private readonly PropertyInfo _referenceProperty;
        private readonly PropertyInfo? _collidesOnProperty;

        public Type EntityType => typeof(TEntity);
        public string PropertyName { get; }
        public string? CollidesOnPropertyName { get; }
        public DataMergeBehaviourEnum Behaviour { get; }

        internal TypedEntityMergeReference(
            Func<AuxiliumDbContext,
            DbSet<TEntity>> set,
            Expression<Func<TEntity, TKey>> reference,
            DataMergeBehaviourEnum behaviour,
            Expression<Func<TEntity, Guid>>? collidesOn
        )
        {
            _set = set;
            _referenceProperty = PropertyOf(reference);
            _collidesOnProperty = collidesOn is null ? null : PropertyOf(collidesOn);
            Behaviour = behaviour;
            PropertyName = _referenceProperty.Name;
            CollidesOnPropertyName = _collidesOnProperty?.Name;
        }



        public async Task<DataMergeReferenceCounts> CountAsync(AuxiliumDbContext db, Guid duplicateId, Guid survivorId, CancellationToken ct = default)
        {
            long total = await _set(db).Where(References(duplicateId)).LongCountAsync(ct);

            long colliding = _collidesOnProperty is null || total == 0
                ? 0
                : await Colliding(db, duplicateId, survivorId).LongCountAsync(ct);

            return new DataMergeReferenceCounts(total, colliding);
        }



        public async Task<DataMergeReferenceChange> ApplyAsync(AuxiliumDbContext db, Guid duplicateId, Guid survivorId, CancellationToken ct = default)
        {
            DataMergeReferenceChange change = new(
                Entity: TrimEntitySuffix(typeof(TEntity).Name),
                Property: PropertyName,
                Behaviour: Behaviour,
                RowsAffected: 0,
                MovedRowIds: [],
                DiscardedRows: [],
                KeptOnDuplicate: 0
            );

            switch (Behaviour)
            {
                case DataMergeBehaviourEnum.Preserve:
                    {
                        var kept = await _set(db).Where(References(duplicateId)).LongCountAsync(ct);
                        return change with { KeptOnDuplicate = kept };
                    }

                case DataMergeBehaviourEnum.Discard:
                    {
                        List<Guid>? ids = await _set(db)
                            .Where(References(duplicateId))
                            .Select(e => e.Id)
                            .ToListAsync(ct);
                        change.DiscardedRows.AddRange(ids.Select(id => new DataMergeDiscardedRow(id, SnapshotJson: null)));

                        int deleted = await _set(db).Where(References(duplicateId)).ExecuteDeleteAsync(ct);
                        return change with {
                            RowsAffected = deleted,
                        };
                    }

                case DataMergeBehaviourEnum.Repoint:
                    {
                        if (_collidesOnProperty is not null)
                        {
                            List<TEntity> colliding = await Colliding(db, duplicateId, survivorId)
                                .AsNoTracking()
                                .ToListAsync(ct);

                            if (colliding.Count > 0)
                            {
                                List<Guid>? collidingIds = colliding.Select(e => e.Id).ToList();

                                await _set(db)
                                    .Where(e => collidingIds.Contains(e.Id))
                                    .ExecuteDeleteAsync(ct);

                                change.DiscardedRows.AddRange(colliding.Select(e =>
                                    new DataMergeDiscardedRow(
                                        e.Id,
                                        JsonSerializer.Serialize(
                                            e,
                                            SnapshotJsonOptions
                                        )
                                    )
                                ));
                            }
                        }

                        change.MovedRowIds.AddRange(await _set(db)
                            .Where(References(duplicateId))
                            .Select(e => e.Id)
                            .ToListAsync(ct));

                        int moved = await _set(db)
                            .Where(References(duplicateId))
                            .ExecuteUpdateAsync(SetReferenceTo(survivorId), ct);

                        return change with {
                            RowsAffected = moved,
                        };
                    }

                default:
                    throw new InvalidOperationException($"Unknown merge behaviour {Behaviour}.");
            }
        }





        private Expression<Func<TEntity, bool>> References(Guid id)
        {
            Expression<Func<Guid>> captured = () => id;
            (ParameterExpression entity, MemberExpression reference) = Access(_referenceProperty);

            return Expression.Lambda<Func<TEntity, bool>>(
                Expression.Equal(
                    reference,
                    Expression.Convert(
                        captured.Body,
                        typeof(TKey)
                    )
                ),
                entity
            );
        }

        private IQueryable<TEntity> Colliding(AuxiliumDbContext db, Guid duplicateId, Guid survivorId)
        {
            (ParameterExpression innerEntity, MemberExpression innerValue) = Access(_collidesOnProperty!);
            var survivorValues = _set(db)
                .Where(References(survivorId))
                .Select(Expression.Lambda<Func<TEntity, Guid>>(innerValue, innerEntity));

            Expression<Func<IQueryable<Guid>>> captured = () => survivorValues;
            (ParameterExpression outerEntity, MemberExpression outerValue) = Access(_collidesOnProperty!);

            var alreadyOnSurvivor = Expression.Lambda<Func<TEntity, bool>>(
                Expression.Call(
                    typeof(Queryable),
                    nameof(Queryable.Contains),
                    [typeof(Guid)],
                    captured.Body,
                    outerValue
                ),
                outerEntity
            );

            return _set(db).Where(References(duplicateId)).Where(alreadyOnSurvivor);
        }

        private Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> SetReferenceTo(Guid id)
        {
            Expression<Func<Guid>> captured = () => id;
            (ParameterExpression entity, MemberExpression reference) = Access(_referenceProperty);
            ParameterExpression? setters = Expression.Parameter(typeof(SetPropertyCalls<TEntity>), "setters");

            MethodCallExpression? call = Expression.Call(
                setters,
                nameof(SetPropertyCalls<TEntity>.SetProperty),
                [typeof(TKey)],
                Expression.Lambda<Func<TEntity, TKey>>(
                    reference,
                    entity
                ),
                Expression.Convert(
                    captured.Body,
                    typeof(TKey)
                )
            );

            return Expression.Lambda<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>>(call, setters);
        }

        private static (ParameterExpression Entity, MemberExpression Value) Access(PropertyInfo property)
        {
            ParameterExpression entity = Expression.Parameter(typeof(TEntity), "e");
            return (entity, Expression.Property(entity, property));
        }

        private static PropertyInfo PropertyOf(LambdaExpression selector) =>
            selector.Body is MemberExpression { Expression: ParameterExpression, Member: PropertyInfo property }
                ? property
                : throw new ArgumentException(
                    $"A merge reference on {typeof(TEntity).Name} must select a property directly, e.g. e => e.CaseId.",
                    nameof(selector)
                );

        private static string TrimEntitySuffix(string clrName) =>
            clrName.EndsWith("EntityModel", StringComparison.Ordinal)
            ? clrName[..^"EntityModel".Length]
            : clrName;
    }
}
