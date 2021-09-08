using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using MrCMS.Models;
using MrCMS.Website;

namespace MrCMS.Services
{
    public interface IGetSystemInfo
    {
        SystemInfo Get();
    }

    public class GetSystemInfo : IGetSystemInfo
    {
        private readonly IGetDateTimeNow _getDateTimeNow;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public GetSystemInfo(IGetDateTimeNow getDateTimeNow, IWebHostEnvironment hostingEnvironment)
        {
            _getDateTimeNow = getDateTimeNow;
            _hostingEnvironment = hostingEnvironment;
        }
        public SystemInfo Get()
        {
            return new SystemInfo()
            {
                Environment = _hostingEnvironment.EnvironmentName,
                DateTime = DateTime.Now,
                UtcDateTime = DateTime.UtcNow,
                MrCMSDateTimeLocalNow = _getDateTimeNow.LocalNow,
                MrCMSDataTimeUtcNow = _getDateTimeNow.UtcNow,
                OperatingSystemName = Environment.OSVersion.VersionString,
                ServerTimeZone = TimeZoneInfo.Local.StandardName,
                LoadedAssemblies = GetLoadedAssemblies()
            };
        }

        private IList<LoadedAssembly> GetLoadedAssemblies()
        {
            var loadedAssemblies = new List<LoadedAssembly>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var loadedAssemblyModel = new LoadedAssembly
                {
                    FullName = assembly.FullName
                };

                try
                {
                    loadedAssemblyModel.Location = assembly.IsDynamic ? null : assembly.Location;
                    loadedAssemblyModel.IsDebug = assembly.GetCustomAttributes(typeof(DebuggableAttribute), false)
                        .FirstOrDefault() is DebuggableAttribute attribute && attribute.IsJITOptimizerDisabled;

                }
                catch
                {
                    // ignored
                }
                loadedAssemblies.Add(loadedAssemblyModel);
            }

            return loadedAssemblies;
        }
    }
}