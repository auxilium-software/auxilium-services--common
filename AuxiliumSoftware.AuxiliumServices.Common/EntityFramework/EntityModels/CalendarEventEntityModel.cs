using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels
{
    public class CalendarEventEntityModel
    {
        [Key]
        public required Guid Id { get; set; }
        public required DateTime CreatedAtUtc { get; set; }
        public Guid? CreatedByUserId { get; set; }
        public DateTime? LastUpdatedAtUtc { get; set; }
        public Guid? LastUpdatedByUserId { get; set; }





        public Guid CategoryValueId { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }





        public DateTime StartUtc { get; set; }
        public DateTime EndUtc { get; set; }
        public bool IsAllDay { get; set; }





        public Guid? CaseId { get; set; }





        public UserEntityModel? CreatedByUser { get; set; }
        public UserEntityModel? LastUpdatedByUser { get; set; }
        public DataEnumeratorValueEntityModel? Category { get; set; }
        public CaseEntityModel? Case { get; set; }
        public List<CalendarEventInviteEntityModel>? Invites { get; set; }
    }
}
