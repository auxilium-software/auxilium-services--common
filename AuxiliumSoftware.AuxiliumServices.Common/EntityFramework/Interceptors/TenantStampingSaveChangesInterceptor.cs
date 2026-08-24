using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Abstractions;
using AuxiliumSoftware.AuxiliumServices.Common.Tenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Interceptors
{
    public sealed class TenantStampingSaveChangesInterceptor : SaveChangesInterceptor
    {
        private readonly ITenantContext _tenantContext;

        public TenantStampingSaveChangesInterceptor(ITenantContext tenantContext) => this._tenantContext = tenantContext;

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            Stamp(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken ct = default)
        {
            Stamp(eventData.Context);
            return base.SavingChangesAsync(eventData, result, ct);
        }

        private void Stamp(DbContext? context)
        {
            if (context is null)
            {
                return;
            }

            foreach (var entry in context.ChangeTracker.Entries<TenantScopedEntityModel>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        if (!this._tenantContext.IsResolved)
                        {
                            throw new InvalidOperationException($"Cannot insert {entry.Metadata.ClrType.Name} with no resolved tenant.");
                        }

                        entry.Entity.TenantId = this._tenantContext.TenantId;

                        if (entry.Entity.CreatedAtUtc == default)
                        {
                            entry.Entity.CreatedAtUtc = DateTime.UtcNow;
                        }
                        break;

                    case EntityState.Modified or EntityState.Deleted:
                        if (entry.Entity.TenantId != this._tenantContext.TenantId)
                        {
                            throw new InvalidOperationException($"Cross-tenant write blocked on {entry.Metadata.ClrType.Name}.");
                        }

                        entry.Property(e => e.TenantId).IsModified = false;
                        entry.Property(e => e.CreatedAtUtc).IsModified = false;
                        break;
                }
            }
        }
    }
}
