using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels
{
    public class LogUserMergeEventEntityModel : LogMergeEventEntityModelBase
    {

        public required Guid SurvivorUserId { get; set; }


        public required Guid TombstoneUserId { get; set; }


        public required string Justification { get; set; }






        public UserEntityModel? SurvivorUser { get; set; }


        public UserEntityModel? TombstoneUser { get; set; }


        public ICollection<LogUserMergeEventRowChangeEntityModel>? RowChanges { get; set; }
    }
}
