using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using Newtonsoft.Json;

namespace MrCMS.Settings
{
    [Obsolete("Site settings have been moved back into the database")]
    public class AppDataConfigurationProvider
    {
        private readonly Site _site;

        public AppDataConfigurationProvider(Site site)
        {
            _site = site;
        }

        public List<SiteSettingsBase> GetAllSiteSettings()
        {
            var methodInfo = GetType().GetMethodExt("GetSiteSettings");

            return TypeHelper.GetAllConcreteTypesAssignableFrom<SiteSettingsBase>()
                .Select(type => methodInfo.MakeGenericMethod(type).Invoke(this, new object[] { }))
                .OfType<SiteSettingsBase>().Where(x => x != null).ToList();
        }

        private TSettings GetSiteSettings<TSettings>() where TSettings : SiteSettingsBase, new()
        {
            GetSettingsObject<TSettings> settingObject = GetSettingObject<TSettings>();
            if (settingObject.IsNew)
                return null;
            return settingObject.Settings;
        }

        public void MarkAsMigrated(SiteSettingsBase siteSettings)
        {
            string fileLocation = GetFileLocation(siteSettings.GetType());
            File.Move(fileLocation, GetMigratedFileLocation(siteSettings.GetType()));
        }

        private string GetFolder()
        {
            string location = string.Format("~/App_Data/Settings/{0}/", _site.Id);
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

        private GetSettingsObject<TSettings> GetSettingObject<TSettings>() where TSettings : SiteSettingsBase, new()
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
                : new GetSettingsObject<TSettings>(GetNewSettingsObject<TSettings>(), true);
        }

        private TSettings GetNewSettingsObject<TSettings>() where TSettings : SiteSettingsBase, new()
        {
            return new TSettings();
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