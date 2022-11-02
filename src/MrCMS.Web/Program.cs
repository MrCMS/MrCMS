﻿using System;
using System.Diagnostics;
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
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile(
                        "connectionstrings.json", optional: true, reloadOnChange: true);
                })
                .UseStaticWebAssets()
                .ConfigureKestrel(options => { options.ConfigureEndpointDefaults(x => x.UseConnectionLogging()); })
                .UseStartup<Startup>()
                .UseKestrel(options =>
                {
                    options.Limits.MaxRequestBodySize = long.MaxValue;
                    options.Limits.KeepAliveTimeout = TimeSpan.FromSeconds(100);
                });
    }
}