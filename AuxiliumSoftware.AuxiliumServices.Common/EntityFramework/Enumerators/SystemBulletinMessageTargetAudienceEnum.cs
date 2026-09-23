using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SystemBulletinMessageTargetAudienceEnum
    {
        [JsonPropertyName("Everyone")]
        Everyone,

        [JsonPropertyName("LoggedInUsersOnly")]
        LoggedInUsersOnly,

        [JsonPropertyName("PublicOnly")]
        PublicOnly,

        [JsonPropertyName("SingleUserOnly")]
        SingleUserOnly,
    }
}
