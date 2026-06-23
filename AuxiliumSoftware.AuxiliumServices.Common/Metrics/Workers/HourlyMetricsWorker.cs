using AuxiliumSoftware.AuxiliumServices.Common.Enumerators;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.Metrics.Workers
{
    public sealed class HourlyMetricsWorker : MetricCollectionWorker
    {
        private const string Schedule = "0 * * * *"; // every hour on the hour
        protected override MetricCadence Cadence => MetricCadence.Hourly;

        public HourlyMetricsWorker(IServiceScopeFactory scopeFactory, ILogger<HourlyMetricsWorker> logger)
            : base(Schedule, scopeFactory, logger) { }
    }
}
