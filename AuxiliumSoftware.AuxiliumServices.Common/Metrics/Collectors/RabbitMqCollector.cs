using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators;
using AuxiliumSoftware.AuxiliumServices.Common.Messaging.Interfaces;
using AuxiliumSoftware.AuxiliumServices.Common.Metrics.Common;
using AuxiliumSoftware.AuxiliumServices.Common.Metrics.Interfaces;
using RabbitMQ.Client;
using System.Diagnostics;

namespace AuxiliumSoftware.AuxiliumServices.Common.Metrics.Collectors
{
    public sealed class RabbitMqCollector : IMetricCollector
    {
        private readonly IRabbitMqConnectionManager _connectionManager;

        public RabbitMqCollector(IRabbitMqConnectionManager connectionManager) => _connectionManager = connectionManager;

        public async Task<IReadOnlyList<MetricSample>> CollectAsync(CancellationToken ct)
        {
            var queues = _connectionManager.Configuration.Queues;
            var samples = new List<MetricSample>();
            var sw = Stopwatch.StartNew();
            try
            {
                var connection = await _connectionManager.GetConnectionAsync(ct);

                await using (var probe = await connection.CreateChannelAsync(cancellationToken: ct))
                {
                    sw.Stop();
                    samples.Add(new(SystemMetricKeyEnum.RabbitMq_IsReachable, 1));
                    samples.Add(new(SystemMetricKeyEnum.RabbitMq_ProbeLatencyInMs, sw.Elapsed.TotalMilliseconds));
                }

                var workingQueues = new[] { queues.Notifications };
                foreach (var queueName in workingQueues)
                {
                    var depth = await TryGetQueueDepthAsync(connection, queueName, ct);
                    if (depth.HasValue)
                    {
                        samples.Add(new(SystemMetricKeyEnum.RabbitMq_MessagesReadyCount, depth.Value, Label: queueName));
                    }
                }

                var dlqDepth = await TryGetQueueDepthAsync(connection, queues.DeadLetter, ct);
                if (dlqDepth.HasValue)
                {
                    samples.Add(new(SystemMetricKeyEnum.RabbitMq_DeadLetterDepth, dlqDepth.Value));
                }
            }
            catch
            {
                samples.Add(new(SystemMetricKeyEnum.RabbitMq_IsReachable, 0));
            }
            return samples;
        }

        private static async Task<uint?> TryGetQueueDepthAsync(IConnection connection, string queueName, CancellationToken ct)
        {
            try
            {
                await using var ch = await connection.CreateChannelAsync(cancellationToken: ct);
                var ok = await ch.QueueDeclarePassiveAsync(queueName, ct);
                return ok.MessageCount;
            }
            catch
            {
                return null;
            }
        }
    }
}
