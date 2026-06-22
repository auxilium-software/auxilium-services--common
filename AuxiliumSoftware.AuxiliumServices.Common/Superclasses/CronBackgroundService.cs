using Cronos;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AuxiliumSoftware.AuxiliumServices.Common.Superclasses
{
    public abstract class CronBackgroundService : BackgroundService
    {
        private readonly CronExpression _cron;
        private readonly TimeZoneInfo _timeZone;
        private readonly ILogger _logger;

        protected CronBackgroundService(string cronExpression, TimeZoneInfo timeZone, ILogger logger)
        {
            _cron = CronExpression.Parse(cronExpression);
            _timeZone = timeZone;
            _logger = logger;
        }

        protected abstract Task DoWorkAsync(CancellationToken stoppingToken);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var next = _cron.GetNextOccurrence(DateTimeOffset.Now, _timeZone);
                if (next is null)
                    break; // no future occurrences

                var delay = next.Value - DateTimeOffset.Now;
                if (delay > TimeSpan.Zero)
                {
                    try
                    {
                        await Task.Delay(delay, stoppingToken);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                }

                try
                {
                    await DoWorkAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Scheduled job {Job} failed", GetType().Name);
                }
            }
        }
    }
}
