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
        private readonly Site _site;
        private static IList<Setting> _allSettings;
        private IDictionary<string, KeyValuePair<int, string>> _allSettingsDictionary;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="session">ISession for db access</param>
        /// <param name="site"></param>
        public SettingService(ISession session, Site site)
        {
            _session = session;
            _site = site;
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
        public virtual Setting GetSettingByKey(string key)
        {
            if (String.IsNullOrEmpty(key))
                return null;

            key = key.Trim().ToLowerInvariant();

            var settings = GetAllSettings();
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
        public virtual T GetSettingValueByKey<T>(string key, T defaultValue = default(T))
        {
            if (String.IsNullOrEmpty(key))
                return defaultValue;

            key = key.Trim().ToLowerInvariant();

            var settings = GetAllSettings();
            if (settings.ContainsKey(key))
                return settings[key].Value.To<T>();

            return defaultValue;
        }

        /// <summary>
        /// Set setting value
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        public virtual void SetSetting<T>(string key, T value)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            key = key.Trim().ToLowerInvariant();

            Setting setting =
                _session.QueryOver<Setting>().Where(s => s.Site == _site && s.Name == key).SingleOrDefault();
            string valueStr = typeof(T).GetCustomTypeConverter().ConvertToInvariantString(value);
            if (setting != null)
            {
                //update
                setting.Value = valueStr;
                setting.Site = _site;
            }
            else
            {
                //insert
                setting = new Setting
                {
                    Name = key,
                    Value = valueStr,
                    Site = _site
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

        public void ResetSettingCache()
        {
            _allSettings = null;
        }

        /// <summary>
        /// Gets all settings
        /// </summary>
        /// <returns>Setting collection</returns>
        private IDictionary<string, KeyValuePair<int, string>> GetAllSettings()
        {
            return _allSettingsDictionary ?? (_allSettingsDictionary = GetSettingsDictionary());
        }

        private IDictionary<string, KeyValuePair<int, string>> GetSettingsDictionary()
        {
            //format: <name, <id, value>>
            var dictionary = new Dictionary<string, KeyValuePair<int, string>>();
            foreach (var s in AllSettings.Where(setting => setting.Site.Id == _site.Id))
            {
                var resourceName = s.Name.ToLowerInvariant();
                if (!dictionary.ContainsKey(resourceName))
                    dictionary.Add(resourceName, new KeyValuePair<int, string>(s.Id, s.Value));
            }
            return dictionary;
        }

        private IList<Setting> AllSettings
        {
            get
            {
                return
                    _allSettings =
                    _allSettings ??
                    GetAllSettingForSite();
            }
        }

        private IList<Setting> GetAllSettingForSite()
        {
            return _session.QueryOver<Setting>().Cacheable().List();
        }
    }
}