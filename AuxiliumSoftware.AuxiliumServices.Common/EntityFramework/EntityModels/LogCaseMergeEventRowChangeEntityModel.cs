using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Abstractions;
using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels
{
    public class LogCaseMergeEventRowChangeEntityModel : TenantScopedEntityModelBase
    {
        public required Guid CaseMergeEventId { get; set; }





        public required string EntityName { get; set; }
        public required string PropertyName { get; set; }
        public required Guid RowId { get; set; }
        public required DataMergeRowActionEnum Action { get; set; }
        public string? SnapshotJson { get; set; }





        public LogCaseMergeEventEntityModel? CaseMergeEvent { get; set; }
    }
}
