using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.Tenancy.TenantLookup
{
    public sealed class TenantLookup : ITenantLookup
    {
        private readonly AuxiliumDbContext _db;

        public TenantLookup(AuxiliumDbContext db)
        {
            this._db = db;
        }

        public async Task<TenantLookupResult?> FindByDomainAsync(string domain, CancellationToken ct = default)
        {
            // tenants__tenants has no query filter, so this is safe to run before a tenant is resolved
            return await this._db.Global_Tenants
                .AsNoTracking()
                .Where(t => t.Domain == domain)
                .Select(t => new TenantLookupResult(t.Id, t.LifecycleStatus))
                .FirstOrDefaultAsync(ct);
        }
    }
}
