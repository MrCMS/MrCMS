using System.Collections.Generic;
using MrCMS.Entities.Multisite;
using Ninject.Activation;

namespace MrCMS.Settings
{
    public interface IConfigurationProvider
    {
        TSettings GetSiteSettings<TSettings>() where TSettings : SiteSettingsBase, new();
        void SaveSettings(SiteSettingsBase settings);
        void DeleteSettings(SiteSettingsBase settings);
        List<SiteSettingsBase> GetAllSiteSettings();

        TSettings GetGlobalSettings<TSettings>() where TSettings : GlobalSettingsBase, new();
        void SaveSettings(GlobalSettingsBase settings);
        void DeleteSettings(GlobalSettingsBase settings);
        List<GlobalSettingsBase> GetAllGlobalSettings();
    }
}