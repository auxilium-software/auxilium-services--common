using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Abstractions
{
    public abstract class MutableTenantScopedEntityModel : TenantScopedEntityModel
    {
        public Guid? CreatedByUserId { get; set; }
        public DateTime? LastUpdatedAtUtc { get; set; }
        public Guid? LastUpdatedByUserId { get; set; }





        public UserEntityModel? CreatedByUser { get; set; }
        public UserEntityModel? LastUpdatedByUser { get; set; }
    }
}
