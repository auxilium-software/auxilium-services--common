using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels
{
    public class CalendarEventInviteEntityModel
    {
        [Key]
        public required Guid Id { get; set; }
        public required DateTime CreatedAtUtc { get; set; }
        public Guid? CreatedByUserId { get; set; }
        public DateTime? LastUpdatedAtUtc { get; set; }
        public Guid? LastUpdatedByUserId { get; set; }





        public Guid CalendarEventId { get; set; }

        public Guid InvitedUserId { get; set; }

        public CalendarEventInviteStatusEnum Status { get; set; }

        public Guid InvitedByUserId { get; set; }

        public DateTime InvitedAtUtc { get; set; }

        public DateTime? RespondedAtUtc { get; set; }




        public UserEntityModel? CreatedByUser { get; set; }
        public UserEntityModel? LastUpdatedByUser { get; set; }
        public CalendarEventEntityModel? CalendarEvent { get; set; }
        public UserEntityModel? InvitedUser { get; set; }
        public UserEntityModel? InvitedByUser { get; set; }
    }
}
