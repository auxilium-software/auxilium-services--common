using System;
using System.Collections.Generic;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.Tenancy.TenantLookup
{
    public interface ITenantLookup
    {
        Task<TenantLookupResult?> FindByDomainAsync(string domain, CancellationToken ct = default);
    }
}
