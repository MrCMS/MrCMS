using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace MrCMS.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args).ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile(
                        "connectionStrings.json", optional: true, reloadOnChange: true);
                })
                .UseStartup<Startup>();
    }
}
