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
    [Obsolete("System settings have been moved into the web.config file.")]
    public class AppDataSystemConfigurationProvider
    {
        public List<SystemSettingsBase> GetAllSystemSettings()
        {
            MethodInfo methodInfo = GetType().GetMethodExt("GetSystemSettings");

            return TypeHelper.GetAllConcreteTypesAssignableFrom<SystemSettingsBase>()
                .Select(type => methodInfo.MakeGenericMethod(type).Invoke(this, new object[] { }))
                .OfType<SystemSettingsBase>().ToList();
        }

        public TSettings GetSystemSettings<TSettings>() where TSettings : SystemSettingsBase, new()
        {
            GetSettingsObject<TSettings> settingObject = GetSettingObject<TSettings>();
            return settingObject.IsNew ? null : settingObject.Settings;
        }

        public string GetFolder()
        {
            string location = string.Format("~/App_Data/Settings/system/");
            string mapPath = HostingEnvironment.MapPath(location);
            Directory.CreateDirectory(mapPath);
            return mapPath;
        }

        private string GetFileLocation(Type type)
        {
            return string.Format("{0}{1}.json", GetFolder(), type.FullName.ToLower());
        }
        private string GetMigratedFileLocation(Type type)
        {
            return string.Format("{0}{1}.migrated", GetFolder(), type.FullName.ToLower());
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
                : new GetSettingsObject<TSettings>(null, true);
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

        public void MarkAsMigrated(SystemSettingsBase settings)
        {
            string fileLocation = GetFileLocation(settings.GetType());
            File.Move(fileLocation, GetMigratedFileLocation(settings.GetType()));
        }
    }

}