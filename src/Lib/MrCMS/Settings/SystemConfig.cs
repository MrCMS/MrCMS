using System;
using System.Collections.Generic;

namespace MrCMS.Settings
{
    /// <summary>
    /// Mr CMS System settings from appsettings.json
    /// </summary>
    public class SystemConfig
    {
        public List<string> SupportedCultures { get; set; } = new List<string>() {"en-GB"};
        public string TimeZone { get; set; }
        
        public TimeZoneInfo TimeZoneInfo =>
            !string.IsNullOrEmpty(TimeZone) ? TimeZoneInfo.FindSystemTimeZoneById(TimeZone) : TimeZoneInfo.Local;
    }
}