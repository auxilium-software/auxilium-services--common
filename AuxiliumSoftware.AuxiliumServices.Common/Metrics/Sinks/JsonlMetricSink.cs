using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using AuxiliumSoftware.AuxiliumServices.Common.Metrics.Common;
using AuxiliumSoftware.AuxiliumServices.Common.Metrics.Enumerators;
using AuxiliumSoftware.AuxiliumServices.Common.Metrics.Interfaces;
using AuxiliumSoftware.AuxiliumServices.Common.Metrics.Naming;
using Microsoft.Extensions.Configuration;

namespace AuxiliumSoftware.AuxiliumServices.Common.Metrics.Sinks
{
    public sealed class JsonlMetricSink : IMetricSink
    {
        private static readonly JsonSerializerOptions _json = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = false,
        };

        private readonly string _root;
        private readonly ConcurrentDictionary<string, SemaphoreSlim> _gates = new();

        public JsonlMetricSink(IConfiguration config)
        {
            _root = config["FileSystem:RootStorageDirectories:Metrics"];
            Directory.CreateDirectory(_root);
        }

        public async Task WriteAsync(
            IReadOnlyList<MetricRecord> records,
            CancellationToken ct
        )
        {
            foreach (var byKey in records.GroupBy(r => r.Key))
            {
                var dir = Path.Combine(_root, JsonEnumNames<SystemMetricKeyEnum>.Name(byKey.Key));
                Directory.CreateDirectory(dir);

                foreach (var byDate in byKey.GroupBy(r => r.TimestampUtc.Date))
                {
                    var file = Path.Combine(dir, $"{byDate.Key:yyyy-MM-dd}.jsonl");
                    var sb = new StringBuilder();
                    foreach (var r in byDate)
                    {
                        var row = new DiskRow
                        {
                            T = r.TimestampUtc,
                            V = r.Value,
                            L = r.Label is { } lab ? JsonEnumNames<SystemMetricLabelEnum>.Name(lab) : null,
                        };
                        sb.Append(JsonSerializer.Serialize(row, _json)).Append('\n');
                    }
                    await AppendAsync(file, sb.ToString(), ct);
                }
            }
        }

        public async Task<IReadOnlyList<MetricRecord>> ReadLatestAsync(
            SystemMetricKeyEnum key,
            int count,
            CancellationToken ct
        )
        {
            var dir = Path.Combine(_root, JsonEnumNames<SystemMetricKeyEnum>.Name(key));
            if (!Directory.Exists(dir)) return Array.Empty<MetricRecord>();

            var files = Directory.GetFiles(dir, "*.jsonl")
                .OrderByDescending(f => f, StringComparer.Ordinal);

            var lines = new List<string>();
            foreach (var f in files)
            {
                var fileLines = await ReadAllLinesSharedAsync(f, ct);
                lines.InsertRange(0, fileLines);
                if (lines.Count >= count) break;
            }

            var tail = lines.Count > count
                ? lines.GetRange(lines.Count - count, count)
                : lines;

            var records = new List<MetricRecord>(tail.Count);
            foreach (var ln in tail)
                if (Parse(ln, key) is { } rec) records.Add(rec);

            records.Reverse();
            return records;
        }

        public async Task<IReadOnlyList<MetricRecord>> ReadLatestPerKeyAsync(CancellationToken ct)
        {
            if (!Directory.Exists(_root)) return Array.Empty<MetricRecord>();

            var result = new List<MetricRecord>();
            foreach (var dir in Directory.GetDirectories(_root))
            {
                if (!JsonEnumNames<SystemMetricKeyEnum>.TryParse(Path.GetFileName(dir), out var key))
                    continue;

                var files = Directory.GetFiles(dir, "*.jsonl")
                    .OrderByDescending(f => f, StringComparer.Ordinal);

                foreach (var f in files)
                {
                    var fileLines = await ReadAllLinesSharedAsync(f, ct);
                    var last = fileLines.LastOrDefault(l => !string.IsNullOrWhiteSpace(l));
                    if (last is null) continue;
                    if (Parse(last, key) is { } rec) { result.Add(rec); break; }
                }
            }
            return result;
        }

        private static MetricRecord? Parse(string line, SystemMetricKeyEnum key)
        {
            if (string.IsNullOrWhiteSpace(line)) return null;
            try
            {
                var row = JsonSerializer.Deserialize<DiskRow>(line, _json);
                if (row is null) return null;

                SystemMetricLabelEnum? label = null;
                if (row.L is { } l && JsonEnumNames<SystemMetricLabelEnum>.TryParse(l, out var le))
                    label = le;

                return new MetricRecord(key, DateTime.SpecifyKind(row.T, DateTimeKind.Utc), row.V, label);
            }
            catch
            {
                return null;
            }
        }

        private async Task AppendAsync(
            string path,
            string content,
            CancellationToken ct
        )
        {
            var gate = _gates.GetOrAdd(path, _ => new SemaphoreSlim(1, 1));
            await gate.WaitAsync(ct);
            try
            {
                await using var fs = new FileStream(
                    path, FileMode.Append, FileAccess.Write, FileShare.Read);
                await fs.WriteAsync(Encoding.UTF8.GetBytes(content), ct);
            }
            finally
            {
                gate.Release();
            }
        }

        private static async Task<string[]> ReadAllLinesSharedAsync(string path, CancellationToken ct)
        {
            await using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var sr = new StreamReader(fs);
            var content = await sr.ReadToEndAsync(ct);
            return content.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        }
    }
}
