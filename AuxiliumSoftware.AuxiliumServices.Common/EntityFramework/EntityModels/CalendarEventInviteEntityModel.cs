using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators;
using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels
{
    public class CalendarEventInviteEntityModel : IMandatoryFieldsEntityModel
    {
        public Guid? CreatedByUserId { get; set; }
        public DateTime? LastUpdatedAtUtc { get; set; }
        public Guid? LastUpdatedByUserId { get; set; }





        public Guid CalendarEventId { get; set; }

        public Guid? InvitedUserId { get; set; }

        public CalendarEventInviteStatusEnum Status { get; set; }

        public Guid? InvitedByUserId { get; set; }

        public DateTime InvitedAtUtc { get; set; }

        public DateTime? RespondedAtUtc { get; set; }





        public UserEntityModel? CreatedByUser { get; set; }
        public UserEntityModel? LastUpdatedByUser { get; set; }
        public CalendarEventEntityModel? CalendarEvent { get; set; }
        public UserEntityModel? InvitedUser { get; set; }
        public UserEntityModel? InvitedByUser { get; set; }
    }
}
