using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.Settings;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Settings.Events;
using MrCMS.Website;
using Newtonsoft.Json;
using NHibernate;
using NHibernate.Transform;

namespace MrCMS.Settings
{
    public class SqlConfigurationProvider : IConfigurationProvider
    {
        private readonly IStatelessSession _session;
        private readonly IDictionary<string, SettingForCaching> _settings = new Dictionary<string, SettingForCaching>();
        private readonly Site _site;

        /// <summary>
        ///     Ctor
        /// </summary>
        /// <param name="session">NHibernate session</param>
        /// <param name="site">Current site</param>
        public SqlConfigurationProvider(IStatelessSession session, Site site)
        {
            _session = session;
            _site = site;
        }


        /// <summary>
        ///     Get Site Settings of the requested type
        /// </summary>
        /// <typeparam name="TSettings"></typeparam>
        /// <returns></returns>
        public virtual TSettings GetSiteSettings<TSettings>() where TSettings : SiteSettingsBase, new()
        {
            var settings = Activator.CreateInstance<TSettings>();

            foreach (var prop in typeof (TSettings).GetProperties())
            {
                // get properties we can read and write to
                if (!prop.CanRead || !prop.CanWrite)
                    continue;

                var key = typeof (TSettings).FullName + "." + prop.Name;
                var value = GetSettingByKey(key, prop.PropertyType);
                if (value == null)
                    continue;


                //set property
                prop.SetValue(settings, value, null);
            }

            return settings;
        }

        public void SaveSettings(SiteSettingsBase settings)
        {
            var methodInfo = GetType().GetMethods().First(x => x.Name == "SaveSettings" && x.IsGenericMethod);
            var genericMethod = methodInfo.MakeGenericMethod(settings.GetType());
            genericMethod.Invoke(this, new object[] {settings});
        }

        public List<SiteSettingsBase> GetAllSiteSettings()
        {
            var methodInfo = GetType().GetMethodExt("GetSiteSettings");

            return TypeHelper.GetAllConcreteTypesAssignableFrom<SiteSettingsBase>()
                .Select(type => methodInfo.MakeGenericMethod(type).Invoke(this, new object[] {}))
                .OfType<SiteSettingsBase>().ToList();
        }

        /// <summary>
        ///     Save settings object
        /// </summary>
        /// <typeparam name="TSettings">Type</typeparam>
        /// <param name="settings">Setting instance</param>
        public virtual void SaveSettings<TSettings>(TSettings settings) where TSettings : SiteSettingsBase, new()
        {
            var existing = GetSiteSettings<TSettings>();
            _session.Transact(session =>
            {
                /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
                foreach (var prop in typeof (TSettings).GetProperties())
                {
                    // get properties we can read and write to
                    if (!prop.CanRead || !prop.CanWrite)
                        continue;

                    var key = typeof (TSettings).FullName + "." + prop.Name;
                    dynamic value = prop.GetValue(settings, null);
                    SetSetting(key, value ?? "");
                }
            });
            EventContext.Instance.Publish<IOnSavingSiteSettings<TSettings>, OnSavingSiteSettingsArgs<TSettings>>(
                new OnSavingSiteSettingsArgs<TSettings>(settings, existing));
        }

        /// <summary>
        ///     Delete all settings
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        public virtual void DeleteSettings<T>(T settings) where T : SiteSettingsBase, new()
        {
            var settingsToDelete = new List<Setting>();
            var allSettings = _session.QueryOver<Setting>().Where(x => x.Site.Id == _site.Id).List();
            foreach (var prop in typeof (T).GetProperties())
            {
                var key = typeof (T).FullName + "." + prop.Name;
                settingsToDelete.AddRange(
                    allSettings.Where(x => x.Name.Equals(key, StringComparison.InvariantCultureIgnoreCase)));
            }

            foreach (var setting in settingsToDelete)
                DeleteSetting(setting);
        }

        /// <summary>
        ///     Clear cache
        /// </summary>
        public virtual void ClearCache()
        {
            _settings.Clear();
        }

        /// <summary>
        ///     Gets all settings
        /// </summary>
        /// <returns>Settings</returns>
        protected virtual IDictionary<string, SettingForCaching> GetAllSettingsCached()
        {
            if (_settings.Any())
            {
                return _settings;
            }
            SettingForCaching alias = null;
            var settings = _session.QueryOver<Setting>().Where(x => x.Site.Id == _site.Id && !x.IsDeleted)
                .SelectList(builder =>
                {
                    builder.Select(setting => setting.Name).WithAlias(() => alias.Name);
                    builder.Select(setting => setting.Value).WithAlias(() => alias.Value);
                    builder.Select(setting => setting.Id).WithAlias(() => alias.Id);
                    return builder;
                }).TransformUsing(Transformers.AliasToBean<SettingForCaching>())
                .List<SettingForCaching>();

            settings.GroupBy(setting => setting.Name).ForEach(grouping => _settings[grouping.Key] = grouping.First());
            return _settings;
        }

        /// <summary>
        ///     Adds a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        protected virtual void InsertSetting(Setting setting)
        {
            if (setting == null)
                throw new ArgumentNullException("setting");

            _session.Transact(session => session.Insert(setting));

            _settings[setting.Name] = new SettingForCaching
            {
                Id = setting.Id,
                Name = setting.Name,
                Value = setting.Value
            };
        }

        /// <summary>
        ///     Updates a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        protected virtual void UpdateSetting(Setting setting)
        {
            if (setting == null)
                throw new ArgumentNullException("setting");

            _session.Transact(session => session.Update(setting));
            _settings[setting.Name] = new SettingForCaching
            {
                Id = setting.Id,
                Name = setting.Name,
                Value = setting.Value
            };
        }

        /// <summary>
        ///     Deletes a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        protected virtual void DeleteSetting(Setting setting)
        {
            if (setting == null)
                throw new ArgumentNullException("setting");

            _session.Delete(setting);

            _settings.Remove(setting.Name);
        }

        /// <summary>
        ///     Gets a setting by identifier
        /// </summary>
        /// <param name="settingId">Setting identifier</param>
        /// <returns>Setting</returns>
        public virtual Setting GetSettingById(int settingId)
        {
            if (settingId == 0)
                return null;

            return _session.Get<Setting>(settingId);
        }

        /// <summary>
        ///     Get setting value by key
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="type">value type</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns>Setting value</returns>
        protected virtual object GetSettingByKey(string key, Type type, object defaultValue = null)
        {
            if (string.IsNullOrEmpty(key))
                return defaultValue;

            var settings = GetAllSettingsCached();
            key = key.Trim().ToLowerInvariant();
            if (settings.ContainsKey(key))
            {
                var setting = settings[key];
                if (setting != null)
                    return JsonConvert.DeserializeObject(setting.Value, type);
            }

            return defaultValue;
        }

        /// <summary>
        ///     Set setting value
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        protected virtual void SetSetting<T>(string key, T value)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            key = key.Trim().ToLowerInvariant();
            var valueStr = JsonConvert.SerializeObject(value);

            var allSettings = GetAllSettingsCached();
            var settingForCaching = allSettings.ContainsKey(key) ? allSettings[key] : null;
            if (settingForCaching != null)
            {
                //update
                var setting = GetSettingById(settingForCaching.Id);
                setting.Value = valueStr;
                setting.UpdatedOn = CurrentRequestData.Now;
                UpdateSetting(setting);
            }
            else
            {
                //insert
                var setting = new Setting
                {
                    Name = key,
                    Value = valueStr,
                    Site = _site,
                    CreatedOn = CurrentRequestData.Now,
                    UpdatedOn = CurrentRequestData.Now
                };
                InsertSetting(setting);
            }
        }

        protected class SettingForCaching
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Value { get; set; }
        }
    }
}