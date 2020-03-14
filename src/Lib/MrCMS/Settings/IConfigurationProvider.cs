using System.Collections.Generic;
using System.Threading.Tasks;

namespace MrCMS.Settings
{
    public interface IConfigurationProvider
    {
        Task<TSettings> GetSiteSettings<TSettings>() where TSettings : SiteSettingsBase, new();
        Task SaveSettings(SiteSettingsBase settings);
        Task SaveSettings<T>(T settings) where T : SiteSettingsBase, new();
        Task DeleteSettings<T>(T settings) where T : SiteSettingsBase, new();
        List<SiteSettingsBase> GetAllSiteSettings();
    }
}