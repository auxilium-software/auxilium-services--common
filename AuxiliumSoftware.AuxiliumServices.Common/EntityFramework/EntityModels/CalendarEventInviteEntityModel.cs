using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Abstractions;
using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels
{
    public class CalendarEventInviteEntityModel : MutableTenantScopedEntityModel
    {
        public Guid CalendarEventId { get; set; }

        public Guid? InvitedUserId { get; set; }

        public CalendarEventInviteStatusEnum Status { get; set; }

        public Guid? InvitedByUserId { get; set; }

        public DateTime InvitedAtUtc { get; set; }

        public DateTime? RespondedAtUtc { get; set; }





        public CalendarEventEntityModel? CalendarEvent { get; set; }
        public UserEntityModel? InvitedUser { get; set; }
        public UserEntityModel? InvitedByUser { get; set; }
    }
}
