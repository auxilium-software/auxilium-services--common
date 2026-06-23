using System.Diagnostics;
using AuxiliumSoftware.AuxiliumServices.Common.Enumerators;
using AuxiliumSoftware.AuxiliumServices.Common.Metrics.Common;
using AuxiliumSoftware.AuxiliumServices.Common.Metrics.Interfaces;

namespace AuxiliumSoftware.AuxiliumServices.Common.Metrics.Collectors
{
    public sealed class ProcessResourceCollector : IMetricCollector
    {
        private readonly ProcessMetricKeys _keys;

        public ProcessResourceCollector(ProcessMetricKeys keys) => _keys = keys;

        public MetricCadence Cadence => MetricCadence.Minutely;

        public async Task<IReadOnlyList<MetricSample>> CollectAsync(CancellationToken ct)
        {
            using var proc = Process.GetCurrentProcess();

            var startCpu = proc.TotalProcessorTime;
            var sw = Stopwatch.StartNew();
            await Task.Delay(500, ct);
            sw.Stop();
            proc.Refresh();

            var cpuMs = (proc.TotalProcessorTime - startCpu).TotalMilliseconds;
            var cpu = Math.Clamp(cpuMs / (Environment.ProcessorCount * sw.Elapsed.TotalMilliseconds) * 100d, 0d, 100d);

            double memBytes = Environment.WorkingSet;

            var uptime = (DateTime.UtcNow - proc.StartTime.ToUniversalTime()).TotalSeconds;

            return new[]
            {
                new MetricSample(_keys.Cpu, cpu),
                new MetricSample(_keys.Memory, memBytes),
                new MetricSample(_keys.Uptime, uptime),
            };
        }
    }
}
