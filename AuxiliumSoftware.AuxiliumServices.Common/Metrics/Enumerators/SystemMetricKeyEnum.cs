using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.Common.Metrics.Enumerators
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SystemMetricKeyEnum
    {
        /// <summary>
        /// Gets the size of the database in bytes.
        /// </summary>
        [JsonPropertyName("db.size_in_bytes")]
        Db_SizeInBytes,
        /// <summary>
        /// Gets a value indicating whether the database is reachable.
        /// </summary>
        [JsonPropertyName("db.is_reachable")]
        Db_IsReachable,
        /// <summary>
        /// Gets the probe latency of the database in milliseconds.
        /// </summary>
        [JsonPropertyName("db.probe_latency_in_ms")]
        Db_ProbeLatencyInMs,
        /// <summary>
        /// Gets the number of active connections to the database.
        /// </summary>
        [JsonPropertyName("db.active_connections_count")]
        Db_ActiveConnectionsCount,
        /// <summary>
        /// Gets the number of queries executed by the database per second.
        /// </summary>
        [JsonPropertyName("db.queries_per_second")]
        Db_QueriesPerSecond,
        /// <summary>
        /// Gets the number of slow queries executed by the database per hour.
        /// </summary>
        [JsonPropertyName("db.slow_queries_per_hour")]
        Db_SlowQueriesPerHour,
        /// <summary>
        /// Gets the buffer pool hit ratio of the database.
        /// </summary>
        [JsonPropertyName("db.buffer_pool_hit_ratio")]
        Db_BufferPoolHitRatio,





        /// <summary>
        /// Gets the depth of the RabbitMQ queue.
        /// </summary>
        [JsonPropertyName("rabbitmq.queue_depth")]
        RabbitMq_QueueDepth,
        /// <summary>
        /// Gets a value indicating whether RabbitMQ is reachable.
        /// </summary>
        [JsonPropertyName("rabbitmq.is_reachable")]
        RabbitMq_IsReachable,
        /// <summary>
        /// Gets the probe latency of RabbitMQ in milliseconds.
        /// </summary>
        [JsonPropertyName("rabbitmq.probe_latency_in_ms")]
        RabbitMq_ProbeLatencyInMs,
        /// <summary>
        /// Gets the number of messages ready to be delivered in RabbitMQ.
        /// </summary>
        [JsonPropertyName("rabbitmq.messages_ready_count")]
        RabbitMq_MessagesReadyCount,
        /// <summary>
        /// Gets the number of unacknowledged messages in RabbitMQ.
        /// </summary>
        [JsonPropertyName("rabbitmq.messages_unacked_count")]
        RabbitMq_MessagesUnackedCount,
        /// <summary>
        /// Gets the depth of the dead letter queue in RabbitMQ.
        /// </summary>
        [JsonPropertyName("rabbitmq.dead_letter_depth")]
        RabbitMq_DeadLetterDepth,





        /// <summary>
        /// Gets the size of the LFS in bytes.
        /// </summary>
        [JsonPropertyName("lfs.size_in_bytes")]
        Lfs_SizeInBytes,
        /// <summary>
        /// Gets a value indicating whether the LFS is writable.
        /// </summary>
        [JsonPropertyName("lfs.is_writable")]
        Lfs_IsWritable,
        /// <summary>
        /// Gets the probe latency of the LFS in milliseconds.
        /// </summary>
        [JsonPropertyName("lfs.probe_latency_in_ms")]
        Lfs_ProbeLatencyInMs,





        /// <summary>
        /// Gets a value indicating whether the API is reachable, probed externally from the task runner.
        /// </summary>
        [JsonPropertyName("api.is_reachable")]
        Api_IsReachable,
        /// <summary>
        /// Gets the external probe latency to the API in milliseconds.
        /// </summary>
        [JsonPropertyName("api.probe_latency_in_ms")]
        Api_ProbeLatencyInMs,
        /// <summary>
        /// Gets the CPU usage percentage of the API.
        /// </summary>
        [JsonPropertyName("api.cpu_usage_as_percentage")]
        Api_CpuUsageAsPercentage,
        /// <summary>
        /// Gets the memory usage of the API in bytes.
        /// </summary>
        [JsonPropertyName("api.memory_usage_in_bytes")]
        Api_MemoryUsageInBytes,
        /// <summary>
        /// Gets the uptime of the API in seconds.
        /// </summary>
        [JsonPropertyName("api.uptime_in_seconds")]
        Api_UptimeInSeconds,
        /// <summary>
        /// Gets the number of tasks waiting in the thread pool queue for the API.
        /// </summary>
        [JsonPropertyName("api.thread_pool_queue_length")]
        Api_ThreadPoolQueueLength,
        /// <summary>
        /// Gets the number of Gen0 garbage collections that have occurred in the API per minute.
        /// </summary>
        [JsonPropertyName("api.gen0_collections_per_minute")]
        Api_Gen0CollectionsPerMinute,
        /// <summary>
        /// Gets the number of Gen1 garbage collections that have occurred in the API per minute.
        /// </summary>
        [JsonPropertyName("api.gen1_collections_per_minute")]
        Api_Gen1CollectionsPerMinute,
        /// <summary>
        /// Gets the number of Gen2 garbage collections that have occurred in the API per minute.
        /// </summary>
        [JsonPropertyName("api.gen2_collections_per_minute")]
        Api_Gen2CollectionsPerMinute,
        /// <summary>
        /// Gets the percentage of time spent in garbage collection for the API.
        /// </summary>
        [JsonPropertyName("api.time_in_gc_as_percentage")]
        Api_TimeInGcAsPercentage,
        /// <summary>
        /// Gets the number of API requests received per minute.
        /// </summary>
        [JsonPropertyName("api.requests_per_minute")]
        Api_RequestsPerMinute,
        /// <summary>
        /// Gets the number of HTTP 5xx responses returned by the API per minute.
        /// </summary>
        [JsonPropertyName("api.http_5xx_per_minute")]
        Api_Http5xxPerMinute,
        /// <summary>
        /// Gets the number of exceptions thrown by the API per minute.
        /// </summary>
        [JsonPropertyName("api.exceptions_per_minute")]
        Api_ExceptionsPerMinute,
        /// <summary>
        /// Gets the 50th percentile response time of the API in milliseconds.
        /// </summary>
        [JsonPropertyName("api.response_time_p50_in_ms")]
        Api_ResponseTimeP50InMs,
        /// <summary>
        /// Gets the 95th percentile response time of the API in milliseconds.
        /// </summary>
        [JsonPropertyName("api.response_time_p95_in_ms")]
        Api_ResponseTimeP95InMs,
        /// <summary>
        /// Gets the 99th percentile response time of the API in milliseconds.
        /// </summary>
        [JsonPropertyName("api.response_time_p99_in_ms")]
        Api_ResponseTimeP99InMs,






        /// <summary>
        /// Gets the CPU usage percentage of the task runner.
        /// </summary>
        [JsonPropertyName("task_runner.cpu_usage_as_percentage")]
        TaskRunner_CpuUsageAsPercentage,
        /// <summary>
        /// Gets the memory usage of the task runner in bytes.
        /// </summary>
        [JsonPropertyName("task_runner.memory_usage_in_bytes")]
        TaskRunner_MemoryUsageInBytes,
        /// <summary>
        /// Gets the uptime of the task runner in seconds.
        /// </summary>
        [JsonPropertyName("task_runner.uptime_in_seconds")]
        TaskRunner_UptimeInSeconds,
        /// <summary>
        /// Gets the number of tasks waiting in the thread pool queue for the task runner.
        /// </summary>
        [JsonPropertyName("task_runner.thread_pool_queue_length")]
        TaskRunner_ThreadPoolQueueLength,
        /// <summary>
        /// Gets the number of Gen0 garbage collections that have occurred in the task runner per minute.
        /// </summary>
        [JsonPropertyName("task_runner.gen0_collections_per_minute")]
        TaskRunner_Gen0CollectionsPerMinute,
        /// <summary>
        /// Gets the number of Gen1 garbage collections that have occurred in the task runner per minute.
        /// </summary>
        [JsonPropertyName("task_runner.gen1_collections_per_minute")]
        TaskRunner_Gen1CollectionsPerMinute,
        /// <summary>
        /// Gets the number of Gen2 garbage collections that have occurred in the task runner per minute.
        /// </summary>
        [JsonPropertyName("task_runner.gen2_collections_per_minute")]
        TaskRunner_Gen2CollectionsPerMinute,
        /// <summary>
        /// Gets the percentage of time spent in garbage collection for the task runner.
        /// </summary>
        [JsonPropertyName("task_runner.time_in_gc_as_percentage")]
        TaskRunner_TimeInGcAsPercentage,




        /// <summary>
        /// Gets the free disk space in bytes on the host machine.
        /// </summary>
        [JsonPropertyName("host.disk_free_in_bytes")]
        Host_DiskFreeInBytes,
        /// <summary>
        /// Gets the total disk space in bytes on the host machine.
        /// </summary>
        [JsonPropertyName("host.disk_total_in_bytes")]
        Host_DiskTotalInBytes,
    }
}
