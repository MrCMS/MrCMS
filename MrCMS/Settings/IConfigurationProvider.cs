using System.Collections.Generic;
using MrCMS.Entities.Multisite;
using Ninject.Activation;

namespace MrCMS.Settings
{
    public interface IConfigurationProvider
    {
        TSettings GetSiteSettings<TSettings>(Site site = null) where TSettings : SiteSettingsBase, new();
        void SaveSettings(SiteSettingsBase settings);
        void DeleteSettings(SiteSettingsBase settings);
        List<SiteSettingsBase> GetAllSiteSettings(Site site = null);
    }
}