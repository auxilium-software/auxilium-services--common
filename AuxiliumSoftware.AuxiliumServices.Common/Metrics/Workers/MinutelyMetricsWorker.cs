using AuxiliumSoftware.AuxiliumServices.Common.Enumerators;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.Metrics.Workers
{
    public sealed class MinutelyMetricsWorker : MetricCollectionWorker
    {
        private const string Schedule = "* * * * *"; // every minute
        protected override MetricCadence Cadence => MetricCadence.Minutely;

        public MinutelyMetricsWorker(IServiceScopeFactory scopeFactory, ILogger<MinutelyMetricsWorker> logger)
            : base(Schedule, scopeFactory, logger) { }
    }
}
