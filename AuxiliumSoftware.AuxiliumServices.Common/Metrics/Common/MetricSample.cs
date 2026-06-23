using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.Metrics.Common
{
    public readonly record struct MetricSample(
        SystemMetricKeyEnum Key,
        double Value
    );
}
