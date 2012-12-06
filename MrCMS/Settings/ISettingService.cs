using System.Collections.Generic;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.Settings;

namespace MrCMS.Settings
{
    /// <summary>
    /// Setting service interface
    /// </summary>
    public interface ISettingService
    {
        /// <summary>
        /// Gets a setting by identifier
        /// </summary>
        /// <param name="settingId">Setting identifier</param>
        /// <returns>Setting</returns>
        Setting GetSettingById(int settingId);

        /// <summary>
        /// Get setting by key
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Setting object</returns>
        Setting GetSettingByKey(Site site, string key);

        /// <summary>
        /// Get setting value by key
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns>Setting value</returns>
        T GetSettingValueByKey<T>(Site site, string key, T defaultValue = default(T));

        /// <summary>
        /// Set setting value
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        void SetSetting<T>(Site site, string key, T value);

        /// <summary>
        /// Deletes a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        void DeleteSetting(Setting setting);
    }
}