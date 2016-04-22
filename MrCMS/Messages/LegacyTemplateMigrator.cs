using System;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using MrCMS.Entities.Messaging;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Website;
using NHibernate;
using Ninject;

namespace MrCMS.Messages
{
    public static class LegacyTemplateMigrator
    {
        public static void MigrateTemplates(IKernel kernel)
        {
            var settingsFolder = GetTemplatesFolder();
            
            // we will rename migrated files, so once they've all been migrated, there will be no .json files left
            if (!CurrentRequestData.DatabaseIsInstalled || !Directory.Exists(settingsFolder) || !Directory.EnumerateFiles(settingsFolder,"*.json",SearchOption.AllDirectories).Any())
                return;
            var session = kernel.Get<IStatelessSession>();
            CopyOldSystemSettings(session);
            CopyOldSiteSettings(session);
        }

        private static string GetTemplatesFolder()
        {
            return HostingEnvironment.MapPath("~/App_Data/Templates");
        }

        private static void CopyOldSiteSettings(IStatelessSession session)
        {
            var sites = session.QueryOver<Site>().List();
            foreach (var site in sites)
            {
                CopyTemplates(session, site.Id);
            }
        }
        private static void CopyOldSystemSettings(IStatelessSession session)
        {
            CopyTemplates(session, null);
        }

        private static void CopyTemplates(IStatelessSession session, int? siteId)
        {
            var templatesFolder = GetTemplatesFolder();

            var folder = Path.Combine(templatesFolder, siteId.HasValue ? siteId.ToString(): "system");

            var files = Directory.EnumerateFiles(folder, "*.json", SearchOption.AllDirectories);

            session.Transact(statelessSession =>
            {
                foreach (var file in files)
                {
                    var type =
                        TypeHelper.GetAllTypes()
                            .FirstOrDefault(
                                x =>
                                    x.FullName.Equals(Path.GetFileNameWithoutExtension(file),
                                        StringComparison.OrdinalIgnoreCase));
                    if (type == null)
                        continue;

                    statelessSession.Insert(new MessageTemplateData
                    {
                        CreatedOn = CurrentRequestData.Now,
                        UpdatedOn = CurrentRequestData.Now,
                        Data = File.ReadAllText(file),
                        Type = type.FullName,
                        SiteId = siteId
                    });
                    File.Move(file, file + ".migrated");
                }
            });
        }
    }
}