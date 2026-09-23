using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Abstractions
{
    public abstract class LogMergeEventEntityModelBase : TenantScopedEntityModelBase
    {
        public required Guid CreatedByUserId { get; set; }





        public required string FieldResolutionsJson { get; set; }

        public required string SurvivorPreviousValuesJson { get; set; }

        public required string MergedSnapshotJson { get; set; }





        public UserEntityModel? CreatedByUser { get; set; }
    }
}
