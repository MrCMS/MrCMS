using System.Collections.Generic;
using MrCMS.Entities.Multisite;
using Ninject.Activation;

namespace MrCMS.Settings
{
    public interface IConfigurationProvider
    {
        TSettings GetSettings<TSettings>(Site site) where TSettings : SiteSettingsBase, new();
        void SaveSettings(SiteSettingsBase settings);
        void DeleteSettings(Site site, SiteSettingsBase settings);
        List<SiteSettingsBase> GetAllSiteSettings(Site site);

        TSettings GetSettings<TSettings>() where TSettings : GlobalSettingsBase, new();
        void SaveSettings(GlobalSettingsBase settings);
        void DeleteSettings(GlobalSettingsBase settings);
        List<GlobalSettingsBase> GetAllGlobalSettings();
    }
}