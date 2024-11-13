using Microsoft.Win32;
using System.Diagnostics;
using System.Runtime.Versioning;
using System.Threading;

namespace Kapps.TimeZoneKeeper
{
    [SupportedOSPlatform("windows")]
    public class TimeZoneWorker : BackgroundService
    {
        private readonly ILogger<TimeZoneWorker> _logger;

        public TimeZoneWorker(ILogger<TimeZoneWorker> logger)
        {
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            Log("Starting TimeZoneKeeper...");
            SystemEvents.TimeChanged += OnTimeZoneChanged;

            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            Log("Stopping TimeZoneKeeper...");
            SystemEvents.TimeChanged -= OnTimeZoneChanged;

            return base.StopAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            SetTimeZoneIfNeeded();

            return Task.CompletedTask;
        }

        private void OnTimeZoneChanged(object? sender, EventArgs e)
        {
            SetTimeZoneIfNeeded();
        }

        private void SetTimeZoneIfNeeded()
        {
            TimeZoneInfo.ClearCachedData();
            TimeZoneInfo localTimeZone = TimeZoneInfo.Local;

            if (localTimeZone.Id != "Central European Standard Time")
            {
                Log("Current timezone is '" + localTimeZone.DisplayName + "'. Changing to '(UTC+01:00) Sarajevo, Skopje, Warsaw, Zagreb'...");
                Log("Changing to '(UTC+01:00) Sarajevo, Skopje, Warsaw, Zagreb'...");
                try
                {
                    SetTimeZone("Central European Standard Time");
                    Log("Timezone changed successfully to (UTC+01:00) Sarajevo, Skopje, Warsaw, Zagreb.");
                }
                catch (Exception ex)
                {
                    Log("Failed to change timezone: " + ex.Message);
                }
            }
            else
            {
                Log("Timezone is  set to (UTC+01:00) Sarajevo, Skopje, Warsaw, Zagreb. No change needed.");
            }
        }

        private static void SetTimeZone(string timeZoneId)
        {
            var process = Process.Start(new ProcessStartInfo()
            {
                FileName = "tzutil.exe",
                Arguments = "/s \"" + timeZoneId + "\"",
                UseShellExecute = false,
                CreateNoWindow = true
            });

            if (process == null)
            {
                return;
            }

            process.WaitForExit();
            TimeZoneInfo.ClearCachedData();
        }

        private void Log(string message)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("{Time} {Message}", DateTime.Now.ToString(), message);
            }
        }
    }
}
