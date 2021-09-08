using MrCMS.Entities.Multisite;
using MrCMS.Settings;

namespace MrCMS.Tasks
{
    public class GetInternalTaskUrl : IGetInternalTaskUrl
    {
        private readonly IConfigurationProviderFactory _providerFactory;

        public GetInternalTaskUrl(IConfigurationProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
        }

        public string GetSiteTaskUrl<T>(Site site) where T : SiteTask
        {
            var siteSettings = _providerFactory.GetForSite(site).GetSiteSettings<SiteSettings>();
            return
                $"https://{site.BaseUrl.TrimEnd('/')}/{TaskExecutionController.ExecuteTaskURL}?{siteSettings.TaskExecutorKey}={siteSettings.TaskExecutorPassword}&type={typeof(T).FullName}";
        }

        public string GetQueuedTaskUrl(Site site)
        {
            var siteSettings = _providerFactory.GetForSite(site).GetSiteSettings<SiteSettings>();
            return
                $"https://{site.BaseUrl.TrimEnd('/')}/{TaskExecutionController.ExecuteQueuedTasksURL}?{siteSettings.TaskExecutorKey}={siteSettings.TaskExecutorPassword}";
        }
    }
}