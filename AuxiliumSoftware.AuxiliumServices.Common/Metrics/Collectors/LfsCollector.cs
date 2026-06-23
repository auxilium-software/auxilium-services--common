using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators;
using AuxiliumSoftware.AuxiliumServices.Common.Metrics.Interfaces;
using AuxiliumSoftware.AuxiliumServices.Common.Metrics.Common;

namespace AuxiliumSoftware.AuxiliumServices.Common.Metrics.Collectors
{
    public sealed class LfsCollector : IMetricCollector
    {
        private readonly string _root;

        public LfsCollector(IConfiguration config)
            => _root = config["FileSystem:RootStorageDirectories:AuxLFS"]
            ?? throw new InvalidOperationException("FileSystem:RootStorageDirectories:AuxLFS configuration value is missing.");

        public async Task<IReadOnlyList<MetricSample>> CollectAsync(CancellationToken ct)
        {
            var samples = new List<MetricSample>();
            var probe = Path.Combine(_root, $".healthcheck-{Guid.NewGuid():N}");
            var sw = Stopwatch.StartNew();
            try
            {
                await File.WriteAllTextAsync(probe, "ok", ct);
                File.Delete(probe);
                sw.Stop();
                samples.Add(new(SystemMetricKeyEnum.Lfs_IsWritable, 1));
                samples.Add(new(SystemMetricKeyEnum.Lfs_ProbeLatencyInMs, sw.Elapsed.TotalMilliseconds));
            }
            catch
            {
                samples.Add(new(SystemMetricKeyEnum.Lfs_IsWritable, 0));
            }
            return samples;
        }
    }
}
