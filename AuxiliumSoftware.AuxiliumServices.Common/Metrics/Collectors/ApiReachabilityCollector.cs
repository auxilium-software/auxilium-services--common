using AuxiliumSoftware.AuxiliumServices.Common.Configuration.Sections.Databases;
using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators;
using AuxiliumSoftware.AuxiliumServices.Common.Enumerators;
using AuxiliumSoftware.AuxiliumServices.Common.Metrics.Common;
using AuxiliumSoftware.AuxiliumServices.Common.Metrics.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace AuxiliumSoftware.AuxiliumServices.Common.Metrics.Collectors
{
    public sealed class ApiReachabilityCollector : IMetricCollector
    {
        private readonly IHttpClientFactory _httpFactory;
        private readonly string _healthUrl;

        public ApiReachabilityCollector(
            IHttpClientFactory httpFactory,
            IConfiguration config
        )
        {
            _httpFactory = httpFactory;

            var baseUrl = config["API:PrimarilyAvailableAt"] ?? throw new InvalidOperationException("API:PrimarilyAvailableAt configuration value is missing.");
            _healthUrl = baseUrl + "/server/ping";
        }

        public MetricCadence Cadence => MetricCadence.Minutely;

        public async Task<IReadOnlyList<MetricSample>> CollectAsync(CancellationToken ct)
        {
            var client = _httpFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(5);

            var sw = Stopwatch.StartNew();
            try
            {
                var resp = await client.GetAsync(_healthUrl, ct);
                sw.Stop();
                return new[]
                {
                    new MetricSample(SystemMetricKeyEnum.Api_IsReachable, resp.IsSuccessStatusCode ? 1 : 0),
                    new MetricSample(SystemMetricKeyEnum.Api_ProbeLatencyInMs, sw.Elapsed.TotalMilliseconds),
                };
            }
            catch
            {
                return new[] { new MetricSample(SystemMetricKeyEnum.Api_IsReachable, 0) };
            }
        }
    }
}
