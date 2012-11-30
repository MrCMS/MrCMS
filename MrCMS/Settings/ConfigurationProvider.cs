using System;
using System.Linq;
using MrCMS.Helpers;
using Ninject.Activation;

namespace MrCMS.Settings
{
    public class ConfigurationProvider<TSettings> : IConfigurationProvider<TSettings> where TSettings : ISettings, new()
    {
        readonly ISettingService _settingService;

        public ConfigurationProvider(ISettingService settingService)
        {
            _settingService = settingService;
            BuildConfiguration();
        }

        public TSettings Settings { get; protected set; }

        private void BuildConfiguration()
        {
            Settings = Activator.CreateInstance<TSettings>();

            // get properties we can write to
            var properties = from prop in typeof(TSettings).GetProperties()
                             where prop.CanWrite && prop.CanRead
                             let setting = _settingService.GetSettingValueByKey<string>(string.Format("{0}.{1}", typeof(TSettings).FullName, prop.Name))
                             where setting != null
                             where prop.PropertyType.GetCustomTypeConverter().CanConvertFrom(typeof(string))
                             where prop.PropertyType.GetCustomTypeConverter().IsValid(setting)
                             let value = prop.PropertyType.GetCustomTypeConverter().ConvertFromInvariantString(setting)
                             select new { prop, value };

            // assign properties
            properties.ToList().ForEach(p => p.prop.SetValue(Settings, p.value, null));
        }

        public void SaveSettings(TSettings settings)
        {
            var properties = from prop in typeof(TSettings).GetProperties()
                             where prop.CanWrite && prop.CanRead
                             where prop.PropertyType.GetCustomTypeConverter().CanConvertFrom(typeof(string))
                             select prop;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            foreach (var prop in properties)
            {
                string key = typeof(TSettings).FullName + "." + prop.Name;
                //Duck typing is not supported in C#. That's why we're using dynamic type
                dynamic value = prop.GetValue(settings, null);
                if (value != null)
                    _settingService.SetSetting(key, value);
                else
                    _settingService.SetSetting(key, "");
            }

            this.Settings = settings;
        }

        public void DeleteSettings()
        {
            var properties = from prop in typeof(TSettings).GetProperties()
                             select prop;

            var settingList =
                properties.Select(prop => typeof(TSettings).FullName + "." + prop.Name)
                    .Select(key => _settingService.GetSettingByKey(key))
                    .Where(setting => setting != null).ToList();

            foreach (var setting in settingList)
                _settingService.DeleteSetting(setting);
        }

        public object Create(IContext context)
        {
            BuildConfiguration();
            return Settings;
        }

        public Type Type { get; private set; }
    }
}