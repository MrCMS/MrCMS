using System.Collections.Generic;

namespace MrCMS.Settings
{
    public interface IConfigurationProvider
    {
        TSettings GetSiteSettings<TSettings>() where TSettings : SiteSettingsBase, new();
        void SaveSettings(SiteSettingsBase settings);
        void DeleteSettings(SiteSettingsBase settings);
        List<SiteSettingsBase> GetAllSiteSettings();
    }
}