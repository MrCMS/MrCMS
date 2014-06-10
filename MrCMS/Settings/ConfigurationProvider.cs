using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Helpers;
using System.Web.Hosting;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.Settings;
using MrCMS.Helpers;
using Newtonsoft.Json;

namespace MrCMS.Settings
{
    public class ConfigurationProvider : IConfigurationProvider
    {
        //private readonly ISettingService _settingService;
        private readonly Site _site;
        private static readonly object SettingsLockObject = new object();

        private static Dictionary<int, Dictionary<Type, object>> _settingCache =
            new Dictionary<int, Dictionary<Type, object>>();

        public ConfigurationProvider(Site site)
        {
            //_settingService = settingService;
            _site = site;
        }

        public string GetFolder()
        {
            string location = string.Format("~/App_Data/Settings/{0}/", _site.Id);
            string mapPath = HostingEnvironment.MapPath(location);
            Directory.CreateDirectory(mapPath);
            return mapPath;
        }

        private string GetFileLocation(SiteSettingsBase settings)
        {
            return GetFileLocation(settings.GetType());
        }


        private string GetFileLocation(Type type)
        {
            return string.Format("{0}{1}.json", GetFolder(), type.FullName.ToLower());
        }

        public void SaveSettings(SiteSettingsBase settings)
        {
            lock (SettingsLockObject)
            {
                string location = GetFileLocation(settings);
                File.WriteAllText(location, JsonConvert.SerializeObject(settings));
                GetSiteCache()[settings.GetType()] = settings;
            }
        }

        private Dictionary<Type, object> GetSiteCache()
        {
            if (!_settingCache.ContainsKey(_site.Id))
            {
                _settingCache[_site.Id] = new Dictionary<Type, object>();
            }
            return _settingCache[_site.Id];
        }

        public void DeleteSettings(SiteSettingsBase settings)
        {
            string fileLocation = GetFileLocation(settings);
            if (File.Exists(fileLocation))
            {
                File.Delete(fileLocation);
            }
        }

        public List<SiteSettingsBase> GetAllSiteSettings()
        {
            var methodInfo = GetType().GetMethodExt("GetSiteSettings");

            return TypeHelper.GetAllConcreteTypesAssignableFrom<SiteSettingsBase>()
                             .Select(type => methodInfo.MakeGenericMethod(type).Invoke(this, new object[] { }))
                             .OfType<SiteSettingsBase>().ToList();

        }

        public TSettings GetSiteSettings<TSettings>() where TSettings : SiteSettingsBase, new()
        {
            if (!GetSiteCache().ContainsKey(typeof(TSettings)))
            {
                GetSiteCache()[typeof(TSettings)] = GetSettingObject<TSettings>();
            }
            return GetSiteCache()[typeof(TSettings)] as TSettings;
        }

        private TSettings GetSettingObject<TSettings>() where TSettings : SiteSettingsBase, new()
        {
            string fileLocation = GetFileLocation(typeof(TSettings));
            if (File.Exists(fileLocation))
            {
                string readAllText = File.ReadAllText(fileLocation);
                return JsonConvert.DeserializeObject<TSettings>(readAllText);
            }
            return new TSettings();
        }
    }
}