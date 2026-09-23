using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels
{
    public class LogCaseMergeEventEntityModel : LogMergeEventEntityModelBase
    {
        public required Guid SurvivorCaseId { get; set; }

        public required Guid TombstoneCaseId { get; set; }





        public CaseEntityModel? SurvivorCase { get; set; }
        public CaseEntityModel? TombstoneCase { get; set; }





        public ICollection<LogCaseMergeEventRowChangeEntityModel>? RowChanges { get; set; }
    }
}
