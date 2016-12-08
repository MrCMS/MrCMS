using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.DbConfiguration;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.Settings;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Settings.Events;
using MrCMS.Website;
using Newtonsoft.Json;
using NHibernate;
using StackExchange.Profiling;

namespace MrCMS.Settings
{
    public class SqlConfigurationProvider : IConfigurationProvider
    {
        private readonly ISession _session;
        private readonly Site _site;

        /// <summary>
        ///     Ctor
        /// </summary>
        /// <param name="session">NHibernate session</param>
        /// <param name="site">Current site</param>
        public SqlConfigurationProvider(ISession session, Site site)
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
            using (MiniProfiler.Current.Step($"Get site settings: {typeof(TSettings).FullName}"))
            {
                var settings = Activator.CreateInstance<TSettings>();

                var dbSettings = GetDbSettings<TSettings>();

                foreach (var prop in typeof(TSettings).GetProperties())
                {
                    // get properties we can read and write to
                    if (!prop.CanRead || !prop.CanWrite)
                        continue;

                    var value = GetSettingByKey(dbSettings, prop.Name, prop.PropertyType);
                    if (value == null)
                        continue;


                    //set property
                    prop.SetValue(settings, value, null);
                }

                return settings;
            }
        }

        public void SaveSettings(SiteSettingsBase settings)
        {
            var methodInfo = GetType().GetMethods().First(x => (x.Name == "SaveSettings") && x.IsGenericMethod);
            var genericMethod = methodInfo.MakeGenericMethod(settings.GetType());
            genericMethod.Invoke(this, new object[] { settings });
        }

        public List<SiteSettingsBase> GetAllSiteSettings()
        {
            var methodInfo = GetType().GetMethodExt("GetSiteSettings");

            return TypeHelper.GetAllConcreteTypesAssignableFrom<SiteSettingsBase>()
                .Select(type => methodInfo.MakeGenericMethod(type).Invoke(this, new object[] { }))
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
            var existingInDb = GetDbSettings<TSettings>();
            _session.Transact(session =>
            {
                /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
                foreach (var prop in typeof(TSettings).GetProperties())
                {
                    // get properties we can read and write to
                    if (!prop.CanRead || !prop.CanWrite)
                        continue;

                    var typeName = typeof(TSettings).FullName;
                    dynamic value = prop.GetValue(settings, null);
                    SetSetting(existingInDb, typeName, prop.Name, value ?? "");
                }
            });
            EventContext.Instance.Publish<IOnSavingSiteSettings<TSettings>, OnSavingSiteSettingsArgs<TSettings>>(
                new OnSavingSiteSettingsArgs<TSettings>(settings, existing));
        }

        /// <summary>
        ///     Delete all settings
        /// </summary>
        /// <typeparam name="TSettings">Type</typeparam>
        public virtual void DeleteSettings<TSettings>(TSettings settings) where TSettings : SiteSettingsBase, new()
        {
            var allSettings = GetDbSettings<TSettings>().Values;

            foreach (var setting in allSettings)
                DeleteSetting(setting);
        }

        /// <summary>
        ///     Clear cache
        /// </summary>
        public virtual void ClearCache()
        {
        }


        private IDictionary<string, Setting> GetDbSettings<TSettings>() where TSettings : SiteSettingsBase, new()
        {
            using (MiniProfiler.Current.Step($"Get from db: {typeof(TSettings).FullName}"))
            {
                var typeName = typeof(TSettings).FullName.ToLower();
                IList<Setting> settings;
                using (new SiteFilterDisabler(_session))
                {
                    settings =
                        _session.QueryOver<Setting>()
                            .Where(x => x.SettingType == typeName && x.Site.Id == _site.Id)
                            .Cacheable()
                            .List();
                }
                return settings.GroupBy(setting => setting.PropertyName)
                    .ToDictionary(x => x.Key, x => x.Select(y => y).First());
            }
        }

        /// <summary>
        ///     Adds a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        protected virtual void InsertSetting(Setting setting)
        {
            if (setting == null)
                throw new ArgumentNullException("setting");

            _session.Transact(session => session.Save(setting));
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
        }

        /// <summary>
        ///     Get setting value by key
        /// </summary>
        /// <param name="existingSettings"></param>
        /// <param name="propertyName">Key</param>
        /// <param name="type">value type</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns>Setting value</returns>
        protected virtual object GetSettingByKey(IDictionary<string, Setting> existingSettings, string propertyName,
            Type type, object defaultValue = null)
        {
            if (string.IsNullOrEmpty(propertyName))
                return defaultValue;

            propertyName = Standardise(propertyName);
            if (existingSettings.ContainsKey(propertyName))
            {
                var setting = existingSettings[propertyName];
                if (setting != null)
                    return JsonConvert.DeserializeObject(setting.Value, type);
            }

            return defaultValue;
        }

        protected virtual void SetSetting<T>(IDictionary<string, Setting> existingSettings, string typeName,
            string propertyName, T value)
        {
            typeName = Standardise(typeName);
            propertyName = Standardise(propertyName);
            var valueStr = JsonConvert.SerializeObject(value);

            var setting = existingSettings.ContainsKey(propertyName) ? existingSettings[propertyName] : null;
            if (setting != null)
            {
                //update
                setting.Value = valueStr;
                setting.UpdatedOn = CurrentRequestData.Now;
                UpdateSetting(setting);
            }
            else
            {
                //insert
                setting = new Setting
                {
                    SettingType = typeName,
                    PropertyName = propertyName,
                    Value = valueStr,
                    Site = _site,
                    CreatedOn = CurrentRequestData.Now,
                    UpdatedOn = CurrentRequestData.Now
                };
                InsertSetting(setting);
            }
        }

        private string Standardise(string value)
        {
            return value?.Trim().ToLowerInvariant();
        }
    }
}