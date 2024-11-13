using System.Runtime.Versioning;

namespace Kapps.TimeZoneKeeper
{
    [SupportedOSPlatform("windows")]
    public static class Program
    {
        public const string ServiceName = "KappsTimeZoneWorker";

        public static void Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .UseWindowsService(options => {
                    options.ServiceName = ServiceName;
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<TimeZoneWorker>();
                })
                .Build();

            host.Run();
        }
    }
}