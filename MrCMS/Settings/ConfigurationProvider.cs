using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Hosting;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using Newtonsoft.Json;

namespace MrCMS.Settings
{
    public class ConfigurationProvider : IConfigurationProvider
    {
        private static readonly object SaveLockObject = new object();
        private static readonly object ReadLockObject = new object();

        private static readonly Dictionary<int, Dictionary<Type, object>> _settingCache =
            new Dictionary<int, Dictionary<Type, object>>();

        private readonly ILegacySettingsProvider _legacySettingsProvider;
        private readonly Site _site;

        public ConfigurationProvider(Site site, ILegacySettingsProvider legacySettingsProvider)
        {
            _site = site;
            _legacySettingsProvider = legacySettingsProvider;
        }

        public void SaveSettings(SiteSettingsBase settings)
        {
            if (settings.SiteId <= 0)
            {
                settings.SiteId = _site.Id;
            }
            lock (SaveLockObject)
            {
                string location = GetFileLocation(settings);
                File.WriteAllText(location, JsonConvert.SerializeObject(settings));
                GetSiteCache()[settings.GetType()] = settings;
            }
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
            MethodInfo methodInfo = GetType().GetMethodExt("GetSiteSettings");

            return TypeHelper.GetAllConcreteTypesAssignableFrom<SiteSettingsBase>()
                .Select(type => methodInfo.MakeGenericMethod(type).Invoke(this, new object[] { }))
                .OfType<SiteSettingsBase>().ToList();
        }

        public TSettings GetSiteSettings<TSettings>() where TSettings : SiteSettingsBase, new()
        {
            lock (ReadLockObject)
            {
                if (!GetSiteCache().ContainsKey(typeof(TSettings)))
                {
                    var settingObject = GetSettingObject<TSettings>();
                    SaveSettings(settingObject);
                }
                return GetSiteCache()[typeof(TSettings)] as TSettings;
            }
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

        private Dictionary<Type, object> GetSiteCache()
        {
            if (!_settingCache.ContainsKey(_site.Id))
            {
                _settingCache[_site.Id] = new Dictionary<Type, object>();
            }
            return _settingCache[_site.Id];
        }

        private TSettings GetSettingObject<TSettings>() where TSettings : SiteSettingsBase, new()
        {
            string fileLocation = GetFileLocation(typeof(TSettings));
            if (File.Exists(fileLocation))
            {
                string readAllText = File.ReadAllText(fileLocation);
                return JsonConvert.DeserializeObject<TSettings>(readAllText);
            }
            return GetNewSettingsObject<TSettings>();
        }

        private TSettings GetNewSettingsObject<TSettings>() where TSettings : SiteSettingsBase, new()
        {
            var settings = new TSettings { SiteId = _site.Id };
            _legacySettingsProvider.ApplyLegacySettings(settings, _site.Id);
            return settings;
        }
    }
}