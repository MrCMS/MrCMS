using System;
using System.Collections.Generic;

namespace MrCMS.Models
{
    public record SystemInfo
    {
        public string Environment { get; set; }
        public string ServerTimeZone { get; set; }
        public DateTime UtcDateTime { get; set; }
        public DateTime DateTime { get; set; }
        public DateTime MrCMSDateTimeLocalNow { get; set; }
        public string OperatingSystemName { get; set; }
        public IList<LoadedAssembly> LoadedAssemblies { get; set; }
        public DateTime MrCMSDataTimeUtcNow { get; set; }
    }
    
    public record LoadedAssembly 
    {
        public string FullName { get; set; }
        public string Location { get; set; }
        public bool IsDebug { get; set; }
    }
}