using System;
using System.Collections.Generic;

namespace MrCMS.Settings
{
    /// <summary>
    /// Mr CMS System settings from appsettings.json
    /// </summary>
    public class SystemConfigurationSettings
    {
        public List<string> SupportedCultures { get; set; }
        public string TimeZone { get; set; }
        
        public TimeZoneInfo TimeZoneInfo =>
            !string.IsNullOrEmpty(TimeZone) ? TimeZoneInfo.FindSystemTimeZoneById(TimeZone) : TimeZoneInfo.Local;
    }
}