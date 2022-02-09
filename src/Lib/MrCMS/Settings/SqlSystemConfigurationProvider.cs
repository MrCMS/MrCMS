using MrCMS.Entities.Settings;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Settings.Events;
using Newtonsoft.Json;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Website.Caching;

namespace MrCMS.Settings
{
    public class SqlSystemConfigurationProvider : ISystemConfigurationProvider
    {
        public const string SystemConfigProviderDbKey = "SystemConfigProviderDbKey-{0}";
        private readonly IStatelessSession _session;

        private readonly IEventContext _eventContext;
        private readonly ICacheManager _cacheManager;

        public SqlSystemConfigurationProvider(IStatelessSession session, IEventContext eventContext,
            ICacheManager cacheManager)
        {
            _session = session;
            _eventContext = eventContext;
            _cacheManager = cacheManager;
        }

        public TSettings GetSystemSettings<TSettings>() where TSettings : SystemSettingsBase, new()
        {
            var typeName = typeof(TSettings).FullName.ToLower();

            return _cacheManager.GetOrCreate(string.Format(SystemConfigProviderDbKey, typeName), () =>
            {
                var settings = Activator.CreateInstance<TSettings>();

                var dbSettings = GetDbSettings<TSettings>();

                foreach (var prop in typeof(TSettings).GetProperties())
                {
                    // get properties we can read and write to
                    if (!prop.CanRead || !prop.CanWrite)
                    {
                        continue;
                    }

                    var value = GetSettingByKey(dbSettings, prop.Name, prop.PropertyType);
                    if (value == null)
                    {
                        continue;
                    }


                    //set property
                    prop.SetValue(settings, value, null);
                }

                return settings;
            }, TimeSpan.FromMinutes(15), CacheExpiryType.Sliding);
        }


        private IDictionary<string, SystemSetting> GetDbSettings<TSettings>()
            where TSettings : SystemSettingsBase, new()
        {
            var typeName = typeof(TSettings).FullName.ToLower();


            var settings =
                _session.QueryOver<SystemSetting>()
                    .Where(x => x.SettingType == typeName)
                    .Cacheable()
                    .List();
            return settings.GroupBy(setting => setting.PropertyName)
                .ToDictionary(x => x.Key, x => x.Select(y => y).First());
        }


        /// <summary>
        ///     Get setting value by key
        /// </summary>
        /// <param name="existingSettings"></param>
        /// <param name="propertyName">Key</param>
        /// <param name="type">value type</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns>Setting value</returns>
        protected virtual object GetSettingByKey(IDictionary<string, SystemSetting> existingSettings,
            string propertyName,
            Type type, object defaultValue = null)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return defaultValue;
            }

            propertyName = Standardise(propertyName);
            if (existingSettings.ContainsKey(propertyName))
            {
                var setting = existingSettings[propertyName];
                if (setting != null)
                {
                    return JsonConvert.DeserializeObject(setting.Value, type);
                }
            }

            return defaultValue;
        }

        private string Standardise(string value)
        {
            return value?.Trim().ToLowerInvariant();
        }

        public async Task SaveSettings(SystemSettingsBase settings)
        {
            var methodInfo = GetType().GetMethods().First(x => x.Name == "SaveSettings" && x.IsGenericMethod);
            var genericMethod = methodInfo.MakeGenericMethod(settings.GetType());
            await (Task) genericMethod.Invoke(this, new object[] {settings});

            ClearCache();
        }

        public async Task SaveSettings<TSettings>(TSettings settings) where TSettings : SystemSettingsBase, new()
        {
            var existing = GetSystemSettings<TSettings>();
            var existingInDb = GetDbSettings<TSettings>();
            await _session.TransactAsync(async session =>
            {
                /* We do not clear cache after each setting update.
                 * This behavior can increase performance because cached settings will not be cleared 
                 * and loaded from database after each update */
                foreach (var prop in typeof(TSettings).GetProperties())
                {
                    // get properties we can read and write to
                    if (!prop.CanRead || !prop.CanWrite)
                    {
                        continue;
                    }

                    var typeName = typeof(TSettings).FullName;
                    dynamic value = prop.GetValue(settings, null);
                    await SetSetting(existingInDb, typeName, prop.Name, value ?? "");
                }
            });
            ClearCache();

            await _eventContext.Publish<IOnSavingSystemSettings<TSettings>, OnSavingSystemSettingsArgs<TSettings>>(
                new OnSavingSystemSettingsArgs<TSettings>(settings, existing));
        }

        public List<SystemSettingsBase> GetAllSystemSettings()
        {
            var methodInfo = GetType().GetMethodExt(nameof(GetSystemSettings));

            return TypeHelper.GetAllConcreteTypesAssignableFrom<SystemSettingsBase>()
                .Select(type => methodInfo.MakeGenericMethod(type).Invoke(this, new object[] { }))
                .OfType<SystemSettingsBase>().ToList();
        }

        protected virtual async Task SetSetting<T>(IDictionary<string, SystemSetting> existingSettings, string typeName,
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
                await UpdateSetting(setting);
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
                await InsertSetting(setting);
            }

            ClearCache();
        }


        /// <summary>
        ///     Adds a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        /// <exception cref="ArgumentNullException"></exception>
        protected virtual async Task InsertSetting(SystemSetting setting)
        {
            if (setting == null)
            {
                throw new ArgumentNullException(nameof(setting));
            }

            await _session.TransactAsync(session => session.InsertAsync(setting));
            ClearCache();
        }

        /// <summary>
        ///     Updates a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        /// <exception cref="ArgumentNullException"></exception>
        protected virtual async Task UpdateSetting(SystemSetting setting)
        {
            if (setting == null)
            {
                throw new ArgumentNullException(nameof(setting));
            }

            await _session.TransactAsync(session => session.UpdateAsync(setting));
            ClearCache();
        }

        /// <summary>
        ///     Deletes a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        /// <exception cref="ArgumentNullException"></exception>
        protected virtual void DeleteSetting(SystemSetting setting)
        {
            if (setting == null)
            {
                throw new ArgumentNullException(nameof(setting));
            }

            _session.Delete(setting);
            ClearCache();
        }

        private void ClearCache()
        {
            _cacheManager.Clear();
        }
    }
}