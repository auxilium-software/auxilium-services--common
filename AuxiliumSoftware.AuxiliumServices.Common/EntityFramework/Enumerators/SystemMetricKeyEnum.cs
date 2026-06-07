using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SystemMetricKeyEnum
    {
        [JsonPropertyName("db.size_bytes")]
        Db_SizeBytes,

        [JsonPropertyName("lfs.size_bytes")]
        Lfs_SizeBytes,
    }
}
