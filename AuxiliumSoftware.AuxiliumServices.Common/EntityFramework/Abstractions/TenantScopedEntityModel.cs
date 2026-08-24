using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Abstractions
{
    public abstract class TenantScopedEntityModel
    {
        public required Guid Id { get; set; }
        public required Guid TenantId { get; set; }





        public required DateTime CreatedAtUtc { get; set; }





        public TenantEntityModel? Tenant { get; set; }
    }
}
