using System.Collections.Generic;
using MrCMS.Services.Caching;

namespace MrCMS.Settings
{
    public interface IConfigurationProvider : IClearCache
    {
        TSettings GetSiteSettings<TSettings>() where TSettings : SiteSettingsBase, new();
        void SaveSettings(SiteSettingsBase settings);
        void DeleteSettings(SiteSettingsBase settings);
        List<SiteSettingsBase> GetAllSiteSettings();
    }
}