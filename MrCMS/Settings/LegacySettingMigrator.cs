using System;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.Settings;
using MrCMS.Helpers;
using MrCMS.Installation;
using MrCMS.IoC.Modules;
using MrCMS.Website;
using NHibernate;
using Ninject;
using Ninject.Web.Common;

namespace MrCMS.Settings
{
    public static class LegacySettingMigrator
    {
        private static void CopyOldSiteSettings(IKernel kernel)
        {
            var session = kernel.Get<IStatelessSession>();
            var sites = session.QueryOver<Site>().List();
            foreach (var site in sites)
            {
#pragma warning disable 618 // For migration purposes
                var appData = new AppDataConfigurationProvider(site);
#pragma warning restore 618
                var sql = new SqlConfigurationProvider(session, site);
                foreach (var setting in appData.GetAllSiteSettings())
                {
                    sql.SaveSettings(setting);
                    appData.MarkAsMigrated(setting);
                }
            }
        }
        private static void CopyOldSystemSettings()
        {
#pragma warning disable 618 // For migration purposes
            var appData = new AppDataSystemConfigurationProvider();
#pragma warning restore 618
            var appConfig = new AppConfigSystemConfigurationProvider();
            foreach (var setting in appData.GetAllSystemSettings())
            {
                appConfig.SaveSettings(setting);
                appData.MarkAsMigrated(setting);
            }
        }

        public static void MigrateSettings(IKernel kernel)
        {
            var settingsFolder = GetSettingsFolder();
            
            // we will rename migrated files, so once they've all been migrated, there will be no .json files left
            if (!Directory.Exists(settingsFolder) || !Directory.EnumerateFiles(settingsFolder,"*.json",SearchOption.AllDirectories).Any())
                return;
            CopyOldSystemSettings();
            UpdateDbInstalled(kernel);
            CopyOldSiteSettings(kernel);
        }

        private static void UpdateDbInstalled(IKernel kernel)
        {
            CurrentRequestData.DatabaseIsInstalled = true;
            var session = kernel.Get<IStatelessSession>();
            MarkExistingSettingsAsSoftDeleted(session);
        }

        private static void MarkExistingSettingsAsSoftDeleted(IStatelessSession session)
        {
            //load all existing settings and soft delete them, in order to make sure we don't pick up any legacy records
            var settings = session.QueryOver<Setting>().List();
            foreach (var setting in settings)
                setting.IsDeleted = true;
            session.Transact(statelessSession => settings.ForEach(statelessSession.Update));
        }

        private static string GetSettingsFolder()
        {
            return HostingEnvironment.MapPath("~/App_Data/Settings");
        }
    }
}