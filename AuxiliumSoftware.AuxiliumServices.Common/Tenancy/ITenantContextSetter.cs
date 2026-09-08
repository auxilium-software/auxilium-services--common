using System;
using System.Collections.Generic;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.Tenancy
{
    public interface ITenantContextSetter
    {
        void SetTenant(Guid tenantId);
    }
}
