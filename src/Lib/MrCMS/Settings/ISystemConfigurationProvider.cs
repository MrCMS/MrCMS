using System.Collections.Generic;
using System.Threading.Tasks;

namespace MrCMS.Settings
{
    public interface ISystemConfigurationProvider 
    {
        TSettings GetSystemSettings<TSettings>() where TSettings : SystemSettingsBase, new();
        Task SaveSettings(SystemSettingsBase settings);
        Task SaveSettings<TSettings>(TSettings settings) where TSettings : SystemSettingsBase, new();
        List<SystemSettingsBase> GetAllSystemSettings();
    }
}