using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels
{
    public class LogLoginAttemptEventEntityModel
    {
        /// <summary>
        /// The unique identifier for the Log Entry.
        /// </summary>
        [Key]
        public required Guid Id { get; set; }
        /// <summary>
        /// The timestamp of when the Log Entry was created.
        /// </summary>
        public required DateTime CreatedAtUtc { get; set; }



        /// <summary>
        /// What Email Address was attempted during login.
        /// </summary>
        public required string AttemptedEmailAddress { get; set; }
        /// <summary>
        /// If the Email Address belongs to an existing User, the unique identifier of that User. Otherwise, null.
        /// </summary>
        public Guid? TargetUserId{ get; set; }
        /// <summary>
        /// The IP Address from which the login attempt was made.
        /// </summary>
        public required string ClientIpAddress { get; set; }
        /// <summary>
        /// Whether the login attempt was successful.
        /// </summary>
        public required bool WasLoginSuccessful { get; set; }
        /// <summary>
        /// Whether the login attempt was blocked by the Web Application Firewall before a password check.
        /// </summary>
        public required bool WasBlockedByWaf { get; set; }
        /// <summary>
        /// Reason for failure
        /// </summary>
        public LoginAttemptFailureReasonEnum? FailureReason { get; set; }





        /// <summary>
        /// If the Email Address belongs to an existing User, this will be that User. Otherwise, null.
        /// </summary>
        public UserEntityModel? TargetUser { get; set; }
    }
}
