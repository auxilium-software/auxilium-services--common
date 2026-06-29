using AuxiliumSoftware.AuxiliumServices.Common.Metrics.Common;
using AuxiliumSoftware.AuxiliumServices.Common.Metrics.Enumerators;

namespace AuxiliumSoftware.AuxiliumServices.Common.Metrics.Interfaces
{
    public interface IMetricSink
    {
        Task WriteAsync(IReadOnlyList<MetricRecord> records, CancellationToken ct);



        Task<IReadOnlyList<MetricRecord>> ReadLatestAsync(SystemMetricKeyEnum key, int count, CancellationToken ct);



        Task<IReadOnlyList<MetricRecord>> ReadLatestPerKeyAsync(CancellationToken ct);
    }
}
