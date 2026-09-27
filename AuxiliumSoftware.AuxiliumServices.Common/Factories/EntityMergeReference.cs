using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework;
using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Abstractions;
using AuxiliumSoftware.AuxiliumServices.Common.Enumerators;
using AuxiliumSoftware.AuxiliumServices.Common.Interfaces;
using AuxiliumSoftware.AuxiliumServices.Common.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.Factories
{
    public static class EntityMergeReference
    {
        public static IEntityMergeReference Repoint<TEntity>(
            Func<AuxiliumDbContext,
            DbSet<TEntity>> set,
            Expression<Func<TEntity, Guid>> reference,
            Expression<Func<TEntity, Guid>>? collidesOn = null
        ) where TEntity : TenantScopedEntityModelBase
            => new TypedEntityMergeReference<TEntity, Guid>(set, reference, DataMergeBehaviourEnum.Repoint, collidesOn);

        public static IEntityMergeReference Repoint<TEntity>(
            Func<AuxiliumDbContext,
            DbSet<TEntity>> set,
            Expression<Func<TEntity, Guid?>> reference,
            Expression<Func<TEntity, Guid>>? collidesOn = null
        ) where TEntity : TenantScopedEntityModelBase
            => new TypedEntityMergeReference<TEntity, Guid?>(set, reference, DataMergeBehaviourEnum.Repoint, collidesOn);





        public static IEntityMergeReference Preserve<TEntity>(
            Func<AuxiliumDbContext,
            DbSet<TEntity>> set,
            Expression<Func<TEntity, Guid>> reference
        ) where TEntity : TenantScopedEntityModelBase
            => new TypedEntityMergeReference<TEntity, Guid>(set, reference, DataMergeBehaviourEnum.Preserve, null);

        public static IEntityMergeReference Preserve<TEntity>(
            Func<AuxiliumDbContext,
            DbSet<TEntity>> set,
            Expression<Func<TEntity, Guid?>> reference
        ) where TEntity : TenantScopedEntityModelBase
            => new TypedEntityMergeReference<TEntity, Guid?>(set, reference, DataMergeBehaviourEnum.Preserve, null);





        public static IEntityMergeReference Discard<TEntity>(
            Func<AuxiliumDbContext,
            DbSet<TEntity>> set,
            Expression<Func<TEntity, Guid>> reference
        ) where TEntity : TenantScopedEntityModelBase
            => new TypedEntityMergeReference<TEntity, Guid>(set, reference, DataMergeBehaviourEnum.Discard, null);

        public static IEntityMergeReference Discard<TEntity>(
            Func<AuxiliumDbContext,
            DbSet<TEntity>> set,
            Expression<Func<TEntity, Guid?>> reference
        ) where TEntity : TenantScopedEntityModelBase
            => new TypedEntityMergeReference<TEntity, Guid?>(set, reference, DataMergeBehaviourEnum.Discard, null);
    }
}
