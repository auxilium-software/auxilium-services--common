using Microsoft.Extensions.Configuration;
using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators;
using AuxiliumSoftware.AuxiliumServices.Common.Metrics.Interfaces;
using AuxiliumSoftware.AuxiliumServices.Common.Metrics.Common;

namespace AuxiliumSoftware.AuxiliumServices.Common.Metrics.Collectors
{
    public sealed class HostResourceCollector : IMetricCollector
    {
        private readonly string _dataPath;

        public HostResourceCollector(IConfiguration config)
            => _dataPath = config["FileSystem:RootStorageDirectories:AuxLFS"]
               ?? throw new InvalidOperationException(
                   "FileSystem:RootStorageDirectories:AuxLFS configuration value is missing."
               );

        public Task<IReadOnlyList<MetricSample>> CollectAsync(CancellationToken ct)
        {
            var drive = ResolveDrive(_dataPath);

            IReadOnlyList<MetricSample> samples = new[]
            {
                new MetricSample(SystemMetricKeyEnum.Host_DiskFreeInBytes, drive.AvailableFreeSpace),
                new MetricSample(SystemMetricKeyEnum.Host_DiskTotalInBytes, drive.TotalSize),
            };
            return Task.FromResult(samples);
        }

        private static DriveInfo ResolveDrive(string dataPath)
        {
            var full = Path.TrimEndingDirectorySeparator(Path.GetFullPath(dataPath));
            var comparison = OperatingSystem.IsWindows()
                ? StringComparison.OrdinalIgnoreCase
                : StringComparison.Ordinal;

            DriveInfo? best = null;
            var bestLen = -1;

            foreach (var d in DriveInfo.GetDrives())
            {
                if (!d.IsReady) continue;

                var root = Path.TrimEndingDirectorySeparator(d.RootDirectory.FullName);

                bool match =
                    full.Equals(root, comparison)
                    || (
                        full.StartsWith(root, comparison)
                        && full.Length > root.Length
                        && full[root.Length] == Path.DirectorySeparatorChar
                    );

                if (match && root.Length > bestLen)
                {
                    best = d;
                    bestLen = root.Length;
                }
            }

            return best ?? new DriveInfo(Path.GetPathRoot(full) ?? "/");
        }
    }
}
