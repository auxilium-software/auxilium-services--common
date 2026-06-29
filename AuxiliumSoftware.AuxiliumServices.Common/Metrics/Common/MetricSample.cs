using AuxiliumSoftware.AuxiliumServices.Common.Metrics.Enumerators;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.Metrics.Common
{
    public readonly record struct MetricSample(
        SystemMetricKeyEnum Key,
        double Value,
        SystemMetricLabelEnum? Label = null
    );
}
