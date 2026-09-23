using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum LoginAttemptFailureReasonEnum
    {
        [JsonPropertyName("InvalidPassword")]
        InvalidPassword,

        [JsonPropertyName("UserNotFound")]
        UserNotFound,

        [JsonPropertyName("AccountLocked")]
        AccountLocked,

        [JsonPropertyName("IpBlocked")]
        IPBlocked,
    }
}
