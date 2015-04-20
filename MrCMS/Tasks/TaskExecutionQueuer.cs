using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Hosting;
using MrCMS.Entities.Multisite;
using MrCMS.Settings;

namespace MrCMS.Tasks
{
    public static class TaskExecutionQueuer
    {
        public static void Initialize(IEnumerable<Site> sites)
        {
            foreach (Site site in sites)
            {
                Queue(site);
            }
        }

        public static void Queue(Site site)
        {
            if (!HostingEnvironment.IsHosted)
                return;
            HostingEnvironment.QueueBackgroundWorkItem(async x =>
            {
                var siteSettings = new ConfigurationProvider(site, null).GetSiteSettings<SiteSettings>();

                if (siteSettings.SelfExecuteTasks)
                {
                    string url = String.Format("{0}/execute-pending-tasks?{1}={2}", site.GetFullDomain.TrimEnd('/'),
                        siteSettings.TaskExecutorKey,
                        siteSettings.TaskExecutorPassword);
                    await new HttpClient().GetAsync(url, x);
                }

                await Task.Delay(TimeSpan.FromSeconds(10), x);
                Queue(site);
            });
        }
    }
}