using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.Settings;
using MrCMS.Helpers;
using NHibernate;

namespace MrCMS.Settings
{
    public class ConfigurationProvider : IConfigurationProvider
    {
        private readonly ISettingService _settingService;
        private readonly CurrentSite _currentSite;
        private readonly ISession _session;

        public ConfigurationProvider(ISettingService settingService, CurrentSite currentSite, ISession session)
        {
            _settingService = settingService;
            _currentSite = currentSite;
            _session = session;
        }


        public void SaveSettings(SiteSettingsBase settings)
        {
            var type = settings.GetType();
            IEnumerable<PropertyInfo> properties = from prop in type.GetProperties()
                                                   where prop.CanWrite && prop.CanRead
                                                   where prop.Name != "Site"
                                                   where
                                                       prop.PropertyType
                                                           .GetCustomTypeConverter()
                                                           .CanConvertFrom(typeof(string))
                                                   select prop;

            foreach (PropertyInfo prop in properties)
            {
                string key = type.FullName + "." + prop.Name;
                //Duck typing is not supported in C#. That's why we're using dynamic type
                dynamic value = prop.GetValue(settings, null);
                _settingService.SetSetting(settings.Site, key, value ?? "");
            }
        }

        public void DeleteSettings(SiteSettingsBase settings)
        {
            var type = settings.GetType();
            IEnumerable<PropertyInfo> properties = from prop in type.GetProperties()
                                                   select prop;

            List<Setting> settingList =
                properties.Select(prop => type.FullName + "." + prop.Name)
                          .Select(key => _settingService.GetSettingByKey(_currentSite.Site, key))
                          .Where(setting => setting != null).ToList();

            foreach (Setting setting in settingList)
                _settingService.DeleteSetting(setting);
        }

        public List<SiteSettingsBase> GetAllSiteSettings()
        {
            var methodInfo = GetType().GetMethodExt("GetSiteSettings");

            return TypeHelper.GetAllConcreteTypesAssignableFrom<SiteSettingsBase>()
                             .Select(type => methodInfo.MakeGenericMethod(type).Invoke(this, new object[] { }))
                             .OfType<SiteSettingsBase>().ToList();

        }

        public TSettings GetGlobalSettings<TSettings>() where TSettings : GlobalSettingsBase, new()
        {
            var settings = Activator.CreateInstance<TSettings>();

            // get properties we can write to
            var properties = from prop in typeof(TSettings).GetProperties()
                             where prop.CanWrite && prop.CanRead
                             let setting =
                                 _settingService.GetSettingValueByKey<string>(null,
                                 string.Format("{0}.{1}", typeof(TSettings).FullName, prop.Name))
                             where setting != null
                             where prop.PropertyType.GetCustomTypeConverter().CanConvertFrom(typeof(string))
                             where prop.PropertyType.GetCustomTypeConverter().IsValid(setting)
                             let value = prop.PropertyType.GetCustomTypeConverter().ConvertFromInvariantString(setting)
                             select new { prop, value };

            // assign properties
            properties.ToList().ForEach(p => p.prop.SetValue(settings, p.value, null));

            return settings;
        }

        public void SaveSettings(GlobalSettingsBase settings)
        {
            var type = settings.GetType();
            IEnumerable<PropertyInfo> properties = from prop in type.GetProperties()
                                                   where prop.CanWrite && prop.CanRead
                                                   where
                                                       prop.PropertyType.GetCustomTypeConverter()
                                                           .CanConvertFrom(typeof(string))
                                                   select prop;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            foreach (PropertyInfo prop in properties)
            {
                string key = type.FullName + "." + prop.Name;
                //Duck typing is not supported in C#. That's why we're using dynamic type
                dynamic value = prop.GetValue(settings, null);
                _settingService.SetSetting(null, key, value ?? "");
            }
        }

        public void DeleteSettings(GlobalSettingsBase settings)
        {
            var type = settings.GetType();
            IEnumerable<PropertyInfo> properties = from prop in type.GetProperties()
                                                   select prop;

            List<Setting> settingList =
                properties.Select(prop => type.FullName + "." + prop.Name)
                          .Select(key => _settingService.GetSettingByKey(null, key))
                          .Where(setting => setting != null).ToList();

            foreach (Setting setting in settingList)
                _settingService.DeleteSetting(setting);
        }

        public List<GlobalSettingsBase> GetAllGlobalSettings()
        {
            var methodInfo = GetType().GetMethodExt("GetGlobalSettings");

            return TypeHelper.GetAllConcreteTypesAssignableFrom<GlobalSettingsBase>()
                             .Select(type => methodInfo.MakeGenericMethod(type).Invoke(this, new object[] { }))
                             .OfType<GlobalSettingsBase>().ToList();
        }

        public TSettings GetSiteSettings<TSettings>() where TSettings : SiteSettingsBase, new()
        {
            var settings = Activator.CreateInstance<TSettings>();

            // get properties we can write to
            var properties = from prop in typeof(TSettings).GetProperties()
                             where prop.CanWrite && prop.CanRead
                             where prop.Name != "Site"
                             let setting =
                                 _settingService.GetSettingValueByKey<string>(_currentSite.Site,
                                 string.Format("{0}.{1}", typeof(TSettings).FullName, prop.Name))
                             where setting != null
                             where prop.PropertyType.GetCustomTypeConverter().CanConvertFrom(typeof(string))
                             where prop.PropertyType.GetCustomTypeConverter().IsValid(setting)
                             let value = prop.PropertyType.GetCustomTypeConverter().ConvertFromInvariantString(setting)
                             select new { prop, value };

            // assign properties
            properties.ToList().ForEach(p => p.prop.SetValue(settings, p.value, null));
            settings.Site = _currentSite.Site;

            return settings;
        }
    }
}