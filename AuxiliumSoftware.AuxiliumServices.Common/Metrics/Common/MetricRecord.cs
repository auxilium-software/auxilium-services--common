using AuxiliumSoftware.AuxiliumServices.Common.Metrics.Enumerators;

namespace AuxiliumSoftware.AuxiliumServices.Common.Metrics.Common
{
    public readonly record struct MetricRecord(
        SystemMetricKeyEnum Key,
        DateTime TimestampUtc,
        double Value,
        SystemMetricLabelEnum? Label = null
    );
}
