using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels
{
    public class CaseTimelineEntryEntityModel
    {
        /// <summary>
        /// </summary>
        [Key]
        public required Guid Id { get; set; }
        /// <summary>
        /// </summary>
        public required DateTime CreatedAtUtc { get; set; }
        /// <summary>
        /// </summary>
        public Guid? CreatedByUserId { get; set; }
        /// <summary>
        /// </summary>
        public DateTime? LastUpdatedAtUtc { get; set; }
        /// <summary>
        /// </summary>
        public Guid? LastUpdatedByUserId { get; set; }





        /// <summary>
        /// </summary>
        public required Guid CaseId { get; set; }




        /// <summary>
        /// </summary>
        public required DateTime OccuredAt { get; set; }
        /// <summary>
        /// </summary>
        public required string Title { get; set; }
        /// <summary>
        /// </summary>
        public required string Description { get; set; }





        /// <summary>
        /// </summary>
        public UserEntityModel? CreatedByUser { get; set; }
        /// <summary>
        /// </summary>
        public UserEntityModel? LastUpdatedByUser { get; set; }





        /// <summary>
        /// </summary>
        public CaseEntityModel? Case { get; set; }
    }
}
