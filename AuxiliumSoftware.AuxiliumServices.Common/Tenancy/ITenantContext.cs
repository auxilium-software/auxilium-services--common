using System;
using System.Collections.Generic;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.Tenancy
{
    public interface ITenantContext
    {
        Guid TenantId { get; }
        bool IsResolved { get; }
    }
}
