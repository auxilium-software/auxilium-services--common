using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework
{
    public interface ITenantScopedEntityModel
    {
        Guid TenantId { get; set; }
        TenantEntityModel? Tenant { get; set; }
    }
}
