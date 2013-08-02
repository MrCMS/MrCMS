using System;
using System.Collections.Generic;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.Settings;
using MrCMS.Helpers;
using MrCMS.Website;
using NHibernate;
using Ninject;
using System.Linq;

namespace MrCMS.Settings
{
    /// <summary>
    /// Setting manager
    /// </summary>
    public partial class SettingService : ISettingService
    {
        private readonly ISession _session;
        private IList<Setting> _allSettings;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="session">ISession for db access</param>
        public SettingService(ISession session)
        {
            _session = session;
        }

        /// <summary>
        /// Gets a setting by identifier
        /// </summary>
        /// <param name="settingId">Setting identifier</param>
        /// <returns>Setting</returns>
        public virtual Setting GetSettingById(int settingId)
        {
            return _session.Get<Setting>(settingId);
        }

        /// <summary>
        /// Get setting by key
        /// </summary>
        /// <param name="site">Site (null for global parameter)</param>
        /// <param name="key">Key</param>
        /// <returns>Setting object</returns>
        public virtual Setting GetSettingByKey(Site site, string key)
        {
            if (String.IsNullOrEmpty(key))
                return null;

            key = key.Trim().ToLowerInvariant();

            var settings = GetAllSettings(site);
            if (settings.ContainsKey(key))
            {
                var id = settings[key].Key;
                return GetSettingById(id);
            }

            return null;
        }

        /// <summary>
        /// Get setting value by key
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="site">Site (null for global parameter)</param>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns>Setting value</returns>
        public virtual T GetSettingValueByKey<T>(Site site, string key, T defaultValue = default(T))
        {
            if (String.IsNullOrEmpty(key))
                return defaultValue;

            key = key.Trim().ToLowerInvariant();

            var settings = GetAllSettings(site);
            if (settings.ContainsKey(key))
                return settings[key].Value.To<T>();

            return defaultValue;
        }

        /// <summary>
        /// Set setting value
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="site">Site (null for global parameter)</param>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        public virtual void SetSetting<T>(Site site, string key, T value)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            key = key.Trim().ToLowerInvariant();

            var settings = GetAllSettings(site);

            Setting setting = null;
            string valueStr = typeof(T).GetCustomTypeConverter().ConvertToInvariantString(value);
            if (settings.ContainsKey(key))
            {
                //update
                var settingId = settings[key].Key;
                setting = GetSettingById(settingId);
                setting.Value = valueStr;
                setting.Site = site;
            }
            else
            {
                //insert
                setting = new Setting
                {
                    Name = key,
                    Value = valueStr,
                    Site = site
                };
                AllSettings.Add(setting);
            }
            _session.Transact(session => session.SaveOrUpdate(setting));
        }

        /// <summary>
        /// Deletes a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        public virtual void DeleteSetting(Setting setting)
        {
            if (setting == null)
                throw new ArgumentNullException("setting");

            _session.Transact(session => session.Delete(setting));
        }

        /// <summary>
        /// Gets all settings
        /// </summary>
        /// <param name="site">Site (null for global parameter)</param>
        /// <returns>Setting collection</returns>
        private IDictionary<string, KeyValuePair<int, string>> GetAllSettings(Site site)
        {
            var settings = AllSettings.Where(setting => setting.Site.Id == site.Id).ToList();
            //format: <name, <id, value>>
            var dictionary = new Dictionary<string, KeyValuePair<int, string>>();
            foreach (var s in settings)
            {
                var resourceName = s.Name.ToLowerInvariant();
                if (!dictionary.ContainsKey(resourceName))
                    dictionary.Add(resourceName, new KeyValuePair<int, string>(s.Id, s.Value));
            }
            return dictionary;
        }

        private IList<Setting> AllSettings
        {
            get { return _allSettings = _allSettings ?? _session.QueryOver<Setting>().Cacheable().List(); }
        }

    }
}