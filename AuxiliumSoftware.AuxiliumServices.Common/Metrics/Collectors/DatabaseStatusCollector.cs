using System.Data;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework;
using AuxiliumSoftware.AuxiliumServices.Common.Metrics.Interfaces;
using AuxiliumSoftware.AuxiliumServices.Common.Metrics.Common;
using AuxiliumSoftware.AuxiliumServices.Common.Metrics.Enumerators;

namespace AuxiliumSoftware.AuxiliumServices.Common.Metrics.Collectors
{
    public sealed class DatabaseStatusCollector : IMetricCollector
    {
        private readonly IServiceScopeFactory _scopeFactory;

        private long _questions, _slow;
        private DateTime _last;
        private bool _primed;

        public DatabaseStatusCollector(IServiceScopeFactory scopeFactory) => _scopeFactory = scopeFactory;

        public async Task<IReadOnlyList<MetricSample>> CollectAsync(CancellationToken ct)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AuxiliumDbContext>();
            var conn = db.Database.GetDbConnection();
            var samples = new List<MetricSample>();

            var sw = Stopwatch.StartNew();
            try
            {
                if (conn.State != ConnectionState.Open)
                    await conn.OpenAsync(ct);

                using (var ping = conn.CreateCommand())
                {
                    ping.CommandText = "SELECT 1";
                    await ping.ExecuteScalarAsync(ct);
                }
                sw.Stop();

                samples.Add(new(SystemMetricKeyEnum.Db_IsReachable, 1));
                samples.Add(new(SystemMetricKeyEnum.Db_ProbeLatencyInMs, sw.Elapsed.TotalMilliseconds));

                using (var sizeCmd = conn.CreateCommand())
                {
                    sizeCmd.CommandText = """
                        SELECT CAST(SUM(data_length + index_length) AS SIGNED)
                        FROM information_schema.tables
                        WHERE table_schema = DATABASE()
                        """;
                    var result = await sizeCmd.ExecuteScalarAsync(ct);
                    if (result is not null and not DBNull)
                        samples.Add(new(SystemMetricKeyEnum.Db_SizeInBytes, Convert.ToDouble(result)));
                }

                var status = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SHOW GLOBAL STATUS";
                    using var r = await cmd.ExecuteReaderAsync(ct);
                    while (await r.ReadAsync(ct))
                        if (double.TryParse(r.GetString(1), out var v))
                            status[r.GetString(0)] = v;
                }
                double Get(string k) => status.TryGetValue(k, out var v) ? v : 0d;

                samples.Add(new(SystemMetricKeyEnum.Db_ActiveConnectionsCount, Get("Threads_connected")));

                var reqs = Get("Innodb_buffer_pool_read_requests");
                var reads = Get("Innodb_buffer_pool_reads");
                if (reqs > 0)
                    samples.Add(new(SystemMetricKeyEnum.Db_BufferPoolHitRatio, (1 - reads / reqs) * 100d));

                var now = DateTime.UtcNow;
                long questions = (long)Get("Questions");
                long slow = (long)Get("Slow_queries");
                if (_primed)
                {
                    var secs = (now - _last).TotalSeconds;
                    if (secs > 0)
                        samples.Add(new(SystemMetricKeyEnum.Db_QueriesPerSecond, (questions - _questions) / secs));
                    samples.Add(new(SystemMetricKeyEnum.Db_SlowQueriesPerHour, slow - _slow));
                }
                _questions = questions; _slow = slow; _last = now; _primed = true;
            }
            catch
            {
                samples.Add(new(SystemMetricKeyEnum.Db_IsReachable, 0));
            }
            return samples;
        }
    }
}
