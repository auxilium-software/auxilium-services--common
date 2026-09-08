using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.Tenancy.TenantLookup
{
    public sealed record TenantLookupResult(
        Guid TenantId,
        TenantLifecycleStatusEnum LifecycleStatus
    );
}
