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
        private readonly Site _currentSite;

        public ConfigurationProvider(ISettingService settingService, Site currentSite)
        {
            _settingService = settingService;
            _currentSite = currentSite;
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
                          .Select(key => _settingService.GetSettingByKey(_currentSite, key))
                          .Where(setting => setting != null).ToList();

            foreach (Setting setting in settingList)
                _settingService.DeleteSetting(setting);
        }

        public List<SiteSettingsBase> GetAllSiteSettings(Site site = null)
        {
            var methodInfo = GetType().GetMethodExt("GetSiteSettings", typeof(Site));

            return TypeHelper.GetAllConcreteTypesAssignableFrom<SiteSettingsBase>()
                             .Select(type => methodInfo.MakeGenericMethod(type).Invoke(this, new object[] { site ?? _currentSite }))
                             .OfType<SiteSettingsBase>().ToList();

        }

        public TSettings GetSiteSettings<TSettings>(Site site = null) where TSettings : SiteSettingsBase, new()
        {
            var settings = Activator.CreateInstance<TSettings>();

            // get properties we can write to
            site = site ?? _currentSite;
            var properties = from prop in typeof(TSettings).GetProperties()
                             where prop.CanWrite && prop.CanRead
                             where prop.Name != "Site"
                             let setting =
                                 _settingService.GetSettingValueByKey<string>(site,
                                 string.Format("{0}.{1}", typeof(TSettings).FullName, prop.Name))
                             where setting != null
                             where prop.PropertyType.GetCustomTypeConverter().CanConvertFrom(typeof(string))
                             where prop.PropertyType.GetCustomTypeConverter().IsValid(setting)
                             let value = prop.PropertyType.GetCustomTypeConverter().ConvertFromInvariantString(setting)
                             select new { prop, value };

            // assign properties
            properties.ToList().ForEach(p => p.prop.SetValue(settings, p.value, null));
            settings.Site = site;

            return settings;
        }
    }
}