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
    }
}