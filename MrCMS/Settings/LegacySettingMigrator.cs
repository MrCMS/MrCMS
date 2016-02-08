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
            MarkExistingSettingsAsSoftDeleted(session);
            var sites = session.QueryOver<Site>().List();
            foreach (var site in sites)
            {
                var appDataConfigurationProvider = new AppDataConfigurationProvider(site);
                var sqlConfigurationProvider = new SqlConfigurationProvider(session, site);
                foreach (var setting in appDataConfigurationProvider.GetAllSiteSettings())
                {
                    sqlConfigurationProvider.SaveSettings(setting);
                    appDataConfigurationProvider.DeleteSettings(setting);
                }
            }
        }

        private static void MarkExistingSettingsAsSoftDeleted(IStatelessSession session)
        {
            //load all existing settings and soft delete them, in order to make sure we don't pick up any legacy records
            var settings = session.QueryOver<Setting>().List();
            foreach (var setting in settings)
                setting.IsDeleted = true;
            session.Transact(statelessSession => settings.ForEach(statelessSession.Update));
        }

        private static void CopyOldSystemSettings()
        {
            var appData = new AppDataSystemConfigurationProvider();
            var appConfig = new AppConfigSystemConfigurationProvider();
            foreach (var setting in appData.GetAllSystemSettings())
            {
                appConfig.SaveSettings(setting);
                appData.DeleteSettings(setting);
            }
        }

        public static void MigrateSettings(IKernel kernel)
        {
            var settingsFolder = GetSettingsFolder();

            if (!Directory.Exists(settingsFolder))
                return;
            CopyOldSystemSettings();
            UpdateDbInstalled(kernel);
            CopyOldSiteSettings(kernel);

            // remove folder and all children to prevent future checks
            Directory.Delete(settingsFolder, true);
        }

        private static void UpdateDbInstalled(IKernel kernel)
        {
            CurrentRequestData.DatabaseIsInstalled = true;
        }

        private static string GetSettingsFolder()
        {
            return HostingEnvironment.MapPath("~/App_Data/Settings");
        }
    }
}