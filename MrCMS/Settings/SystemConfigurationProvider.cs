using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Hosting;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Services.Caching;
using MrCMS.Settings.Events;
using Newtonsoft.Json;

namespace MrCMS.Settings
{
    public class SystemConfigurationProvider : ISystemConfigurationProvider
    {
        private static readonly object SaveLockObject = new object();
        private static readonly object ReadLockObject = new object();

        private static readonly Dictionary<Type, object> _settingCache =
            new Dictionary<Type, object>();

        public void SaveSettings(SystemSettingsBase settings)
        {
            MethodInfo methodInfo = GetType().GetMethods().First(x => x.Name == "SaveSettings" && x.IsGenericMethod);
            MethodInfo genericMethod = methodInfo.MakeGenericMethod(settings.GetType());
            genericMethod.Invoke(this, new object[] { settings });
        }

        public void SaveSettings<T>(T settings) where T : SystemSettingsBase, new()
        {
            lock (SaveLockObject)
            {
                GetSettingsObject<T> existing = GetSettingObject<T>();
                string location = GetFileLocation(settings);
                File.WriteAllText(location, settings.Serialize());
                _settingCache[settings.GetType()] = settings;
                EventContext.Instance.Publish<IOnSavingSystemSettings<T>, OnSavingSystemSettingsArgs<T>>(new OnSavingSystemSettingsArgs<T>(settings,
                    existing.Settings));
            }
        }

        public void DeleteSettings(SystemSettingsBase settings)
        {
            string fileLocation = GetFileLocation(settings);
            if (File.Exists(fileLocation))
            {
                File.Delete(fileLocation);
            }
        }

        public List<SystemSettingsBase> GetAllSystemSettings()
        {
            MethodInfo methodInfo = GetType().GetMethodExt("GetSystemSettings");

            return TypeHelper.GetAllConcreteTypesAssignableFrom<SystemSettingsBase>()
                .Select(type => methodInfo.MakeGenericMethod(type).Invoke(this, new object[] { }))
                .OfType<SystemSettingsBase>().ToList();
        }

        public TSettings GetSystemSettings<TSettings>() where TSettings : SystemSettingsBase, new()
        {
            lock (ReadLockObject)
            {
                if (!_settingCache.ContainsKey(typeof(TSettings)))
                {
                    GetSettingsObject<TSettings> settingObject = GetSettingObject<TSettings>();
                    TSettings settings = settingObject.Settings;
                    if (settingObject.IsNew)
                        SaveSettings(settings);
                    else
                        _settingCache[settings.GetType()] = settings;
                }
                return _settingCache[typeof(TSettings)] as TSettings;
            }
        }

        public string GetFolder()
        {
            string location = string.Format("~/App_Data/Settings/system/");
            string mapPath = HostingEnvironment.MapPath(location);
            Directory.CreateDirectory(mapPath);
            return mapPath;
        }

        private string GetFileLocation(SystemSettingsBase settings)
        {
            return GetFileLocation(settings.GetType());
        }


        private string GetFileLocation(Type type)
        {
            return string.Format("{0}{1}.json", GetFolder(), type.FullName.ToLower());
        }


        private GetSettingsObject<TSettings> GetSettingObject<TSettings>() where TSettings : SystemSettingsBase, new()
        {
            string fileLocation = GetFileLocation(typeof(TSettings));
            TSettings result = null;
            if (File.Exists(fileLocation))
            {
                string readAllText = File.ReadAllText(fileLocation);
                result = JsonConvert.DeserializeObject<TSettings>(readAllText);
            }
            return result != null
                ? new GetSettingsObject<TSettings>(result)
                : new GetSettingsObject<TSettings>(new TSettings(), true);
        }

        public static void ClearCache()
        {
            _settingCache.Clear();
        }

        void IClearCache.ClearCache()
        {
            ClearCache();
        }
        private struct GetSettingsObject<TSettings> where TSettings : SystemSettingsBase, new()
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