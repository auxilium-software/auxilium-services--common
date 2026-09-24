using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PasswordSetTokenReasonEnum
    {
        [JsonPropertyName("NewAccount")]
        NewAccount,

        [JsonPropertyName("PasswordReset")]
        PasswordReset,

        [JsonPropertyName("PasswordExpired")]
        PasswordExpired,

        [JsonPropertyName("Auxilium1BcryptMigration")]
        Auxilium1BCryptMigration
    }
}
