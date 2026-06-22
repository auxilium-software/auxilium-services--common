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





        [JsonPropertyName("task_runner.cpu_usage_percentage")]
        TaskRunner_CpuUsagePercentage,
        [JsonPropertyName("task_runner.memory_usage_bytes")]
        TaskRunner_MemoryUsageBytes,





        [JsonPropertyName("api.cpu_usage_percentage")]
        Api_CpuUsagePercentage,
        [JsonPropertyName("api.memory_usage_bytes")]
        Api_MemoryUsageBytes,
    }
}
