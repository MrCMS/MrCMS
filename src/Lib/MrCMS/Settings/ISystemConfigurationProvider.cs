using System.Collections.Generic;
using MrCMS.Services.Caching;

namespace MrCMS.Settings
{
    public interface ISystemConfigurationProvider 
    {
        TSettings GetSystemSettings<TSettings>() where TSettings : SystemSettingsBase, new();
        void SaveSettings(SystemSettingsBase settings);
        void SaveSettings<TSettings>(TSettings settings) where TSettings : SystemSettingsBase, new();
        List<SystemSettingsBase> GetAllSystemSettings();
    }
}