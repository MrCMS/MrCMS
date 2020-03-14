using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Services.Caching;

namespace MrCMS.Settings
{
    public interface ISystemConfigurationProvider 
    {
        Task<TSettings> GetSystemSettings<TSettings>() where TSettings : SystemSettingsBase, new();
        void SaveSettings(SystemSettingsBase settings);
        Task SaveSettings<TSettings>(TSettings settings) where TSettings : SystemSettingsBase, new();
        List<SystemSettingsBase> GetAllSystemSettings();
    }
}