using AuxiliumSoftware.AuxiliumServices.Common.Enumerators;
using AuxiliumSoftware.AuxiliumServices.Common.Metrics.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.Metrics.Interfaces
{
    public interface IMetricCollector
    {
        MetricCadence Cadence => MetricCadence.Hourly;

        Task<IReadOnlyList<MetricSample>> CollectAsync(CancellationToken ct);
    }
}
