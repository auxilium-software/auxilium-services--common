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





        [JsonPropertyName("task_runner.cpu_usage")]
        TaskRunner_CpuUsage,
        [JsonPropertyName("task_runner.memory_usage")]
        TaskRunner_MemoryUsage,





        [JsonPropertyName("api.cpu_usage")]
        Api_CpuUsage,
        [JsonPropertyName("api.memory_usage")]
        Api_MemoryUsage,
    }
}
