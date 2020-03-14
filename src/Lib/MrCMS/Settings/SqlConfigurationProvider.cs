using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MrCMS.Data;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.Settings;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Settings.Events;
using MrCMS.Website;
using Newtonsoft.Json;

namespace MrCMS.Settings
{
    public class SqlConfigurationProvider : IConfigurationProvider
    {
        private readonly IGlobalRepository<Setting> _repository;
        private readonly IGetSiteId _getSiteId;
        private readonly IEventContext _eventContext;

        /// <summary>
        ///     Ctor
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="getSiteId"></param>
        /// <param name="eventContext"></param>
        public SqlConfigurationProvider(IGlobalRepository<Setting> repository, IGetSiteId getSiteId, IEventContext eventContext)
        {
            _repository = repository;
            _getSiteId = getSiteId;
            _eventContext = eventContext;
        }

        /// <summary>
        ///     Get Site Settings of the requested type
        /// </summary>
        /// <typeparam name="TSettings"></typeparam>
        /// <returns></returns>
        public virtual async Task<TSettings> GetSiteSettings<TSettings>() where TSettings : SiteSettingsBase, new()
        {
            //using (MiniProfiler.Current.Step($"Get site settings: {typeof(TSettings).FullName}"))
            {
                var settings = Activator.CreateInstance<TSettings>();

                var dbSettings = await GetDbSettings<TSettings>();

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

        public Task SaveSettings(SiteSettingsBase settings)
        {
            var methodInfo = GetType().GetMethods().First(x => (x.Name == nameof(SaveSettings)) && x.IsGenericMethod);
            var genericMethod = methodInfo.MakeGenericMethod(settings.GetType());
            return (Task)genericMethod.Invoke(this, new object[] { settings });
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
        public virtual async Task SaveSettings<TSettings>(TSettings settings) where TSettings : SiteSettingsBase, new()
        {
            var existing = await GetSiteSettings<TSettings>();
            var existingInDb = await GetDbSettings<TSettings>();
            await _repository.Transact(async (repo, ct) =>
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
                    await SetSetting(existingInDb, typeName, prop.Name, value ?? "");
                }
            });
            await _eventContext.Publish<IOnSavingSiteSettings<TSettings>, OnSavingSiteSettingsArgs<TSettings>>(
                new OnSavingSiteSettingsArgs<TSettings>(settings, existing));
        }

        /// <summary>
        ///     Delete all settings
        /// </summary>
        /// <typeparam name="TSettings">Type</typeparam>
        public virtual async Task DeleteSettings<TSettings>(TSettings settings) where TSettings : SiteSettingsBase, new()
        {
            var dbSettings = await GetDbSettings<TSettings>();
            var allSettings = dbSettings.Values;

            foreach (var setting in allSettings)
                await DeleteSetting(setting);
        }

        /// <summary>
        ///     Clear cache
        /// </summary>
        public virtual void ClearCache()
        {
        }


        private async Task<IDictionary<string, Setting>> GetDbSettings<TSettings>() where TSettings : SiteSettingsBase, new()
        {
            //using (MiniProfiler.Current.Step($"Get from db: {typeof(TSettings).FullName}"))
            {
                var typeName = typeof(TSettings).FullName.ToLower();
                var siteId = _getSiteId.GetId();
                var settings = await _repository.Query()
                    .Where(x => x.SettingType == typeName && x.SiteId == siteId)
                    .ToListAsync();
                return settings.GroupBy(setting => setting.PropertyName)
                    .ToDictionary(x => x.Key, x => x.Select(y => y).First());
            }
        }

        /// <summary>
        ///     Adds a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        /// <exception cref="ArgumentNullException"></exception>
        protected virtual async Task InsertSetting(Setting setting)
        {
            if (setting == null)
                throw new ArgumentNullException(nameof(setting));

            setting.SiteId = _getSiteId.GetId();
            await _repository.Add(setting);
        }

        /// <summary>
        ///     Updates a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        /// <exception cref="ArgumentNullException"></exception>
        protected virtual async Task UpdateSetting(Setting setting)
        {
            if (setting == null)
                throw new ArgumentNullException(nameof(setting));

            await _repository.Update(setting);
        }

        /// <summary>
        ///     Deletes a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        /// <exception cref="ArgumentNullException"></exception>
        protected virtual async Task DeleteSetting(Setting setting)
        {
            if (setting == null)
                throw new ArgumentNullException(nameof(setting));

            await _repository.Delete(setting);
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

        protected virtual async Task SetSetting<T>(IDictionary<string, Setting> existingSettings, string typeName,
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
                setting = new Setting
                {
                    SettingType = typeName,
                    PropertyName = propertyName,
                    Value = valueStr,
                    SiteId = _getSiteId.GetId(),
                    CreatedOn = DateTime.UtcNow,
                    UpdatedOn = DateTime.UtcNow
                };
                await InsertSetting(setting);
            }
        }

        private string Standardise(string value)
        {
            return value?.Trim().ToLowerInvariant();
        }
    }
}