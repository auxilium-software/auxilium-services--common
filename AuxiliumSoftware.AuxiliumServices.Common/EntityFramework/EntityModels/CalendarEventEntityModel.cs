using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels
{
    public class CalendarEventEntityModel : MutableTenantScopedEntityModel
    {
        public Guid? CategoryValueId { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }





        public DateTime StartUtc { get; set; }
        public DateTime EndUtc { get; set; }
        public bool IsAllDay { get; set; }





        public Guid? CaseId { get; set; }





        public DataEnumeratorValueEntityModel? Category { get; set; }
        public CaseEntityModel? Case { get; set; }
        public List<CalendarEventInviteEntityModel>? Invites { get; set; }
    }
}
