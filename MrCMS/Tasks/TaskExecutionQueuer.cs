using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Hosting;
using MrCMS.Entities.Multisite;
using MrCMS.Settings;
using MrCMS.Website;

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
                try
                {
                    var siteSettings = new ConfigurationProvider(site, null).GetSiteSettings<SiteSettings>();

                    if (siteSettings.SelfExecuteTasks)
                    {
                        string url = String.Format("{0}/{1}?{2}={3}",
                            site.GetFullDomain.TrimEnd('/'),
                            TaskExecutionController.ExecutePendingTasksURL,
                            siteSettings.TaskExecutorKey,
                            siteSettings.TaskExecutorPassword);
                        await new HttpClient().GetAsync(url, x);
                    }
                    await Task.Delay(TimeSpan.FromSeconds(siteSettings.TaskExecutionDelay), x);
                }
                finally
                {
                    Queue(site);
                }
            });
        }
    }
}