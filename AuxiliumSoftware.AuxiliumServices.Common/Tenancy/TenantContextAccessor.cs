using System;
using System.Collections.Generic;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.Tenancy
{
    public sealed class TenantContextAccessor : ITenantContext, ITenantContextSetter
    {
        private Guid? _tenantId;

        public bool IsResolved => this._tenantId.HasValue;

        public Guid TenantId => this._tenantId ?? throw new InvalidOperationException("No tenant has been resolved for this scope. Tenant resolution must run before any database access.");

        public void SetTenant(Guid tenantId)
        {
            if (tenantId == Guid.Empty)
                throw new ArgumentException("Tenant id cannot be empty.", nameof(tenantId));

            if (this._tenantId.HasValue && this._tenantId.Value != tenantId)
                throw new InvalidOperationException("The tenant for this scope has already been set and cannot be changed.");

            this._tenantId = tenantId;
        }
    }
}
