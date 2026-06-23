using AuxiliumSoftware.AuxiliumServices.Common.Enumerators;
using AuxiliumSoftware.AuxiliumServices.Common.Metrics.Common;
using AuxiliumSoftware.AuxiliumServices.Common.Metrics.Interfaces;

namespace AuxiliumSoftware.AuxiliumServices.Common.Metrics.Collectors
{
    public sealed class RuntimeCollector : IMetricCollector
    {
        private readonly RuntimeMetricKeys _keys;

        private int _g0, _g1, _g2;
        private TimeSpan _gcTime;
        private DateTime _last;
        private bool _primed;

        public RuntimeCollector(RuntimeMetricKeys keys) => _keys = keys;

        public MetricCadence Cadence => MetricCadence.Minutely;

        public Task<IReadOnlyList<MetricSample>> CollectAsync(CancellationToken ct)
        {
            var now = DateTime.UtcNow;
            int g0 = GC.CollectionCount(0), g1 = GC.CollectionCount(1), g2 = GC.CollectionCount(2);
            var gcTime = GC.GetTotalPauseDuration();

            var samples = new List<MetricSample>
            {
                new(_keys.ThreadPoolQueueLength, ThreadPool.PendingWorkItemCount),
            };

            if (_primed)
            {
                var secs = (now - _last).TotalSeconds;
                samples.Add(new(_keys.Gen0CollectionsPerMinute, g0 - _g0));
                samples.Add(new(_keys.Gen1CollectionsPerMinute, g1 - _g1));
                samples.Add(new(_keys.Gen2CollectionsPerMinute, g2 - _g2));

                var gcPct = secs > 0 ? (gcTime - _gcTime).TotalSeconds / secs * 100d : 0d;
                samples.Add(new(_keys.TimeInGcPercentage, Math.Clamp(gcPct, 0d, 100d)));
            }

            _g0 = g0; _g1 = g1; _g2 = g2; _gcTime = gcTime; _last = now; _primed = true;
            return Task.FromResult<IReadOnlyList<MetricSample>>(samples);
        }
    }
}
