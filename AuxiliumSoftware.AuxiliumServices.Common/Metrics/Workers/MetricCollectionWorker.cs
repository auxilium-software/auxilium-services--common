using AuxiliumSoftware.AuxiliumServices.Common.Enumerators;
using AuxiliumSoftware.AuxiliumServices.Common.Metrics.Common;
using AuxiliumSoftware.AuxiliumServices.Common.Metrics.Interfaces;
using AuxiliumSoftware.AuxiliumServices.Common.Superclasses;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AuxiliumSoftware.AuxiliumServices.Common.Metrics.Workers
{
    public abstract class MetricCollectionWorker : CronBackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger _logger;

        protected abstract MetricCadence Cadence { get; }

        protected MetricCollectionWorker(string schedule, IServiceScopeFactory scopeFactory, ILogger logger)
            : base(schedule, TimeZoneInfo.Utc, logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task DoWorkAsync(CancellationToken stoppingToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var sink = scope.ServiceProvider.GetRequiredService<IMetricSink>();

            var collectors = scope.ServiceProvider
                .GetServices<IMetricCollector>()
                .Where(c => c.Cadence == Cadence);

            var now = DateTime.UtcNow;
            var records = new List<MetricRecord>();

            foreach (var collector in collectors)
            {
                try
                {
                    foreach (var s in await collector.CollectAsync(stoppingToken))
                    {
                        records.Add(new MetricRecord(s.Key, now, s.Value, s.Label));
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Metric collector {Collector} ({Cadence}) failed", collector.GetType().Name, Cadence);
                }
            }

            if (records.Count == 0)
            {
                return;
            }

            await sink.WriteAsync(records, stoppingToken);
        }
    }
}
