using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Abstractions;
using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels
{
    public class CaseTimelineEntryEntityModel : MutableTenantScopedEntityModel
    {
        /// <summary>
        /// </summary>
        public required Guid CaseId { get; set; }
        /// <summary>
        /// </summary>
        public required CaseTimelineEntryTypeEnum EntryType { get; set; }




        /// <summary>
        /// </summary>
        public required DateTime OccurredAtUtc { get; set; }
        /// <summary>
        /// </summary>
        public required string Title { get; set; }
        /// <summary>
        /// </summary>
        public required string Description { get; set; }





        /// <summary>
        /// </summary>
        public CaseEntityModel? Case { get; set; }
    }
}
