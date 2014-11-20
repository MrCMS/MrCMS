using System.Collections.Generic;
using MrCMS.Services.Caching;

namespace MrCMS.Settings
{
    public interface ISystemConfigurationProvider:IClearCache
    {
        TSettings GetSystemSettings<TSettings>() where TSettings : SystemSettingsBase, new();
        void SaveSettings(SystemSettingsBase settings);
        void DeleteSettings(SystemSettingsBase settings);
        List<SystemSettingsBase> GetAllSystemSettings();
    }
}