using System.Collections.Generic;

namespace MrCMS.Settings
{
    public interface ISystemConfigurationProvider
    {
        TSettings GetSystemSettings<TSettings>() where TSettings : SystemSettingsBase, new();
        void SaveSettings(SystemSettingsBase settings);
        void DeleteSettings(SystemSettingsBase settings);
        List<SystemSettingsBase> GetAllSystemSettings();
    }
}