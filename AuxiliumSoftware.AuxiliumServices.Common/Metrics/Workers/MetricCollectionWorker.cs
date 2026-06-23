using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework;
using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels;
using AuxiliumSoftware.AuxiliumServices.Common.Enumerators;
using AuxiliumSoftware.AuxiliumServices.Common.Superclasses;
using AuxiliumSoftware.AuxiliumServices.Common.Utilities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using AuxiliumSoftware.AuxiliumServices.Common.Metrics.Interfaces;

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
            var db = scope.ServiceProvider.GetRequiredService<AuxiliumDbContext>();

            var collectors = scope.ServiceProvider
                .GetServices<IMetricCollector>()
                .Where(c => c.Cadence == Cadence);

            var now = DateTime.UtcNow;
            var rows = new List<SystemMetricEntityModel>();

            foreach (var collector in collectors)
            {
                try
                {
                    foreach (var s in await collector.CollectAsync(stoppingToken))
                    {
                        rows.Add(new SystemMetricEntityModel
                        {
                            Id = UUIDUtilities.GenerateV5(DatabaseObjectTypeEnum.System_MetricEntry),
                            CreatedAtUtc = now,
                            MetricKey = s.Key,
                            MetricValue = s.Value,
                        });
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Metric collector {Collector} ({Cadence}) failed", collector.GetType().Name, Cadence);
                }
            }

            if (rows.Count == 0) return;

            db.System_Metrics.AddRange(rows);
            await db.SaveChangesAsync(stoppingToken);
        }
    }
}
