using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using MrCMS.DbConfiguration;
using MrCMS.Entities.Settings;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Settings.Events;
using Newtonsoft.Json;
using NHibernate;

namespace MrCMS.Settings
{
    public class SqlSystemConfigurationProvider : ISystemConfigurationProvider
    {
        private readonly IStatelessSession _session;

        private readonly IEventContext _eventContext;

        public SqlSystemConfigurationProvider(IStatelessSession session,IEventContext eventContext)
        {
            _session = session;
            _eventContext = eventContext;
        }

        public void ClearCache()
        {
        }

        public TSettings GetSystemSettings<TSettings>() where TSettings : SystemSettingsBase, new()
        {
            //using (MiniProfiler.Current.Step($"Get {typeof(TSettings).FullName}"))
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
        private IDictionary<string, SystemSetting> GetDbSettings<TSettings>() where TSettings : SystemSettingsBase, new()
        {
            //using (MiniProfiler.Current.Step($"Get from db: {typeof(TSettings).FullName}"))
            {
                var typeName = typeof(TSettings).FullName.ToLower();
                var settings =
                        _session.QueryOver<SystemSetting>()
                            .Where(x => x.SettingType == typeName )
                            .Cacheable()
                            .List();
                return settings.GroupBy(setting => setting.PropertyName)
                    .ToDictionary(x => x.Key, x => x.Select(y => y).First());
            }
        }


        /// <summary>
        ///     Get setting value by key
        /// </summary>
        /// <param name="existingSettings"></param>
        /// <param name="propertyName">Key</param>
        /// <param name="type">value type</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns>Setting value</returns>
        protected virtual object GetSettingByKey(IDictionary<string, SystemSetting> existingSettings, string propertyName,
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

        private string Standardise(string value)
        {
            return value?.Trim().ToLowerInvariant();
        }

        public void SaveSettings(SystemSettingsBase settings)
        {
            var methodInfo = GetType().GetMethods().First(x => x.Name == "SaveSettings" && x.IsGenericMethod);
            var genericMethod = methodInfo.MakeGenericMethod(settings.GetType());
            genericMethod.Invoke(this, new object[] { settings });
        }

        public void SaveSettings<TSettings>(TSettings settings) where TSettings : SystemSettingsBase, new()
        {
            var existing = GetSystemSettings<TSettings>();
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

            _eventContext.Publish<IOnSavingSystemSettings<TSettings>, OnSavingSystemSettingsArgs<TSettings>>(
                new OnSavingSystemSettingsArgs<TSettings>(settings, existing));
        }

        public List<SystemSettingsBase> GetAllSystemSettings()
        {
            var methodInfo = GetType().GetMethodExt(nameof(GetSystemSettings));

            return TypeHelper.GetAllConcreteTypesAssignableFrom<SystemSettingsBase>()
                .Select(type => methodInfo.MakeGenericMethod(type).Invoke(this, new object[] { }))
                .OfType<SystemSettingsBase>().ToList();
        }

        protected virtual void SetSetting<T>(IDictionary<string, SystemSetting> existingSettings, string typeName,
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
                setting.UpdatedOn = DateTime.UtcNow;
                UpdateSetting(setting);
            }
            else
            {
                //insert
                setting = new SystemSetting
                {
                    SettingType = typeName,
                    PropertyName = propertyName,
                    Value = valueStr,
                    CreatedOn = DateTime.UtcNow,
                    UpdatedOn = DateTime.UtcNow
                };
                InsertSetting(setting);
            }
        }


        /// <summary>
        ///     Adds a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        /// <exception cref="ArgumentNullException"></exception>
        protected virtual void InsertSetting(SystemSetting setting)
        {
            if (setting == null)
                throw new ArgumentNullException(nameof(setting));

            _session.Transact(session => session.Insert(setting));
        }

        /// <summary>
        ///     Updates a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        /// <exception cref="ArgumentNullException"></exception>
        protected virtual void UpdateSetting(SystemSetting setting)
        {
            if (setting == null)
                throw new ArgumentNullException(nameof(setting));

            _session.Transact(session => session.Update(setting));
        }

        /// <summary>
        ///     Deletes a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        /// <exception cref="ArgumentNullException"></exception>
        protected virtual void DeleteSetting(SystemSetting setting)
        {
            if (setting == null)
                throw new ArgumentNullException(nameof(setting));

            _session.Delete(setting);
        }

    }
}