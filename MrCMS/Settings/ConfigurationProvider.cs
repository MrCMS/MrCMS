using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Hosting;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Services.Caching;
using MrCMS.Settings.Events;
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
            MethodInfo methodInfo = GetType().GetMethods().First(x => x.Name == "SaveSettings" && x.IsGenericMethod);
            MethodInfo genericMethod = methodInfo.MakeGenericMethod(settings.GetType());
            genericMethod.Invoke(this, new object[] {settings});
        }

        public void SaveSettings<T>(T settings) where T : SiteSettingsBase, new()
        {
            lock (SaveLockObject)
            {
                GetSettingsObject<T> existing = GetSettingObject<T>();
                string location = GetFileLocation(settings);
                File.WriteAllText(location, settings.Serialize());
                GetSiteCache()[settings.GetType()] = settings;
                EventContext.Instance.Publish<IOnSavingSiteSettings<T>, OnSavingSiteSettingsArgs<T>>(new OnSavingSiteSettingsArgs<T>(settings,
                    existing.Settings));
            }
        }

        public void DeleteSettings<T>(T settings) where T : SiteSettingsBase, new()
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
                .Select(type => methodInfo.MakeGenericMethod(type).Invoke(this, new object[] {}))
                .OfType<SiteSettingsBase>().ToList();
        }

        public TSettings GetSiteSettings<TSettings>() where TSettings : SiteSettingsBase, new()
        {
            lock (ReadLockObject)
            {
                if (!GetSiteCache().ContainsKey(typeof (TSettings)))
                {
                    GetSettingsObject<TSettings> settingObject = GetSettingObject<TSettings>();
                    TSettings settings = settingObject.Settings;
                    if (settingObject.IsNew)
                        SaveSettings(settings);
                    else
                        GetSiteCache()[settings.GetType()] = settings;
                }
                return GetSiteCache()[typeof (TSettings)] as TSettings;
            }
        }

        void IClearCache.ClearCache()
        {
            ClearCache();
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

        private GetSettingsObject<TSettings> GetSettingObject<TSettings>() where TSettings : SiteSettingsBase, new()
        {
            string fileLocation = GetFileLocation(typeof (TSettings));
            TSettings result = null;
            if (File.Exists(fileLocation))
            {
                string readAllText = File.ReadAllText(fileLocation);
                result = JsonConvert.DeserializeObject<TSettings>(readAllText);
            }
            return result != null
                ? new GetSettingsObject<TSettings>(result)
                : new GetSettingsObject<TSettings>(GetNewSettingsObject<TSettings>(), true);
        }

        private TSettings GetNewSettingsObject<TSettings>() where TSettings : SiteSettingsBase, new()
        {
            var settings = new TSettings();
            _legacySettingsProvider.ApplyLegacySettings(settings, _site.Id);
            return settings;
        }

        public static void ClearCache()
        {
            _settingCache.Clear();
        }

        private struct GetSettingsObject<TSettings> where TSettings : SiteSettingsBase, new()
        {
            public GetSettingsObject(TSettings settings, bool isNew = false)
                : this()
            {
                Settings = settings;
                IsNew = isNew;
            }

            public TSettings Settings { get; private set; }
            public bool IsNew { get; private set; }
        }
    }
}