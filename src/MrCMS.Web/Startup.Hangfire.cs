using Hangfire;
using Microsoft.AspNetCore.Builder;
using MrCMS.Services.Sitemaps;
using MrCMS.Tasks;
using MrCMS.TextSearch.Services;

namespace MrCMS.Web
{
    public static class HangfireJobs
    {
        public static void RegisterJobs(this IApplicationBuilder app)
        {
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangfireDashboardAuthFilter() },
            });

            RecurringJob.AddOrUpdate<ISitemapService>("ISitemapService.WriteSitemap",
                service => service.WriteSitemap(),
                Cron.Daily(1, 0));

            RecurringJob.AddOrUpdate<IPublishScheduledWebpagesTask>("IPublishScheduledWebpagesTask.Execute",
                service => service.Execute(),
                Cron.Minutely());

            RecurringJob.AddOrUpdate<IClearFormEntries>("IClearFormEntries.Execute",
                service => service.Execute(),
                Cron.Daily(1,0));

            RecurringJob.AddOrUpdate<IDeleteExpiredLogsTask>("IDeleteExpiredLogsTask.Execute",
                service => service.Execute(),
                Cron.Hourly());

            RecurringJob.AddOrUpdate<ISendQueuedMessagesTask>("ISendQueuedMessagesTask.Execute",
                service => service.Execute(),
                Cron.Minutely());

            RecurringJob.AddOrUpdate<IRefreshTextSearchIndex>("IRefreshTextSearchIndex.Refresh",
                service => service.Refresh(),
                Cron.Daily(23, 0));

        }
    }
}