using System;
using System.Net;
using System.Net.Sockets;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace MrCMS.Web
{
    public class Program
    {
        private const int DefaultStartPort = 5000;
        private const int MaxPortAttempts = 100;

        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var builder = WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile(
                        "connectionStrings.json", optional: true, reloadOnChange: true);
                    config.AddEnvironmentVariables();
                })
                .UseStaticWebAssets()
                .ConfigureKestrel(options =>
                {
                    options.ConfigureEndpointDefaults(x => x.UseConnectionLogging());
                })
                .UseStartup<Startup>();

            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (string.Equals(environment, "Development", StringComparison.OrdinalIgnoreCase))
            {
                var httpPort = FindAvailablePort(DefaultStartPort);
                var httpsPort = FindAvailablePort(httpPort + 1);

                builder.UseUrls($"http://localhost:{httpPort}", $"https://localhost:{httpsPort}");

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine();
                Console.WriteLine($"  -> HTTP:  http://localhost:{httpPort}");
                Console.WriteLine($"  -> HTTPS: https://localhost:{httpsPort}");
                Console.WriteLine();
                Console.ResetColor();
            }

            return builder;
        }

        private static int FindAvailablePort(int startPort)
        {
            for (var port = startPort; port < startPort + MaxPortAttempts; port++)
            {
                if (IsPortAvailable(port))
                    return port;
            }

            throw new InvalidOperationException(
                $"No available port found in range {startPort}-{startPort + MaxPortAttempts - 1}");
        }

        private static bool IsPortAvailable(int port)
        {
            try
            {
                using var listener = new TcpListener(IPAddress.Loopback, port);
                listener.Start();
                listener.Stop();
                return true;
            }
            catch (SocketException)
            {
                return false;
            }
        }
    }
}