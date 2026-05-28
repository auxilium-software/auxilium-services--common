using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PasswordSetTokenReasonEnum
    {
        [JsonPropertyName("new_account")]
        NewAccount,

        [JsonPropertyName("password_reset")]
        PasswordReset,

        [JsonPropertyName("password_expired")]
        PasswordExpired,

        [JsonPropertyName("auxilium_1_bcrypt_migration")]
        Auxilium1BCryptMigration
    }
}
