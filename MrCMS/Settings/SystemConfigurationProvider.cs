using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Hosting;
using MrCMS.Helpers;
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
            lock (SaveLockObject)
            {
                string location = GetFileLocation(settings);
                File.WriteAllText(location, JsonConvert.SerializeObject(settings));
                _settingCache[settings.GetType()] = settings;
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
                .Select(type => methodInfo.MakeGenericMethod(type).Invoke(this, new object[] {}))
                .OfType<SystemSettingsBase>().ToList();
        }

        public TSettings GetSystemSettings<TSettings>() where TSettings : SystemSettingsBase, new()
        {
            lock (ReadLockObject)
            {
                if (!_settingCache.ContainsKey(typeof (TSettings)))
                {
                    var settingObject = GetSettingObject<TSettings>();
                    SaveSettings(settingObject);
                }
                return _settingCache[typeof (TSettings)] as TSettings;
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


        private TSettings GetSettingObject<TSettings>() where TSettings : SystemSettingsBase, new()
        {
            string fileLocation = GetFileLocation(typeof (TSettings));
            if (File.Exists(fileLocation))
            {
                string readAllText = File.ReadAllText(fileLocation);
                return JsonConvert.DeserializeObject<TSettings>(readAllText);
            }
            return new TSettings();
        }
    }
}