using System.Diagnostics;
using System.Runtime.Versioning;
using System.ServiceProcess;

namespace Kapps.ServiceRunner
{
    [SupportedOSPlatform("windows")]
    internal static class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"TimeZoneKeeper service runner started.");

            string serviceName = TimeZoneKeeper.Program.ServiceName;

            ServiceController serviceController = new ServiceController(serviceName);

            try
            {
                StopService(serviceController);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            try
            {
                ReinstallService(serviceName);
                StartService(serviceController);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }


            Console.WriteLine($"Done.");
            Console.ReadKey();
        }

        static void StartService(ServiceController serviceController)
        {
            if (serviceController.Status == ServiceControllerStatus.Stopped)
            {
                serviceController.Start();
                serviceController.WaitForStatus(ServiceControllerStatus.Running);
                Console.WriteLine("Service started successfully.");
            }
            else
            {
                Console.WriteLine("Service is already running.");
            }
        }

        static void StopService(ServiceController serviceController)
        {
            if (serviceController.Status == ServiceControllerStatus.Running)
            {
                serviceController.Stop();
                serviceController.WaitForStatus(ServiceControllerStatus.Stopped);
                Console.WriteLine("Service stopped successfully.");
            }
            else
            {
                Console.WriteLine("Service is not running.");
            }
        }

        static void ReinstallService(string serviceName)
        {
            // Uninstall the service
            string uninstallCommand = $"sc delete {serviceName}";
            ExecuteCommand(uninstallCommand);
            Console.WriteLine("Service uninstalled successfully.");

            // Reinstall the service (you may need to specify the correct path to your service executable)
            string installCommand = $"sc create {serviceName} binPath= \"{AppDomain.CurrentDomain.BaseDirectory}Kapps.TimeZoneKeeper.exe\" start=auto";
            ExecuteCommand(installCommand);
            Console.WriteLine("Service reinstalled successfully.");
        }

        static void ExecuteCommand(string command)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c {command}",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            string result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            Console.WriteLine(result);
        }
    }
}
