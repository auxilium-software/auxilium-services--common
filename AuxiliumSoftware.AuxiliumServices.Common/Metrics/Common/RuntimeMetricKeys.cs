using AuxiliumSoftware.AuxiliumServices.Common.Metrics.Enumerators;

namespace AuxiliumSoftware.AuxiliumServices.Common.Metrics.Common
{
    public sealed record RuntimeMetricKeys(
        SystemMetricKeyEnum ThreadPoolQueueLength,
        SystemMetricKeyEnum Gen0CollectionsPerMinute,
        SystemMetricKeyEnum Gen1CollectionsPerMinute,
        SystemMetricKeyEnum Gen2CollectionsPerMinute,
        SystemMetricKeyEnum TimeInGcPercentage
    );
}
