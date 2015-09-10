using System.Net.Http;
using System.Web.Mvc;
using System.Web.Routing;
using MrCMS.Batching.Entities;
using MrCMS.Settings;

namespace MrCMS.Batching
{
    public class ExecuteRequestForNextTask : IExecuteRequestForNextTask
    {
        private readonly SiteSettings _siteSettings;
        private readonly UrlHelper _urlHelper;

        public ExecuteRequestForNextTask(UrlHelper urlHelper, SiteSettings siteSettings)
        {
            _urlHelper = urlHelper;
            _siteSettings = siteSettings;
        }

        public void Execute(BatchRun run)
        {
            var httpClient = new HttpClient();
            var routeValues = new RouteValueDictionary
            {
                {"id", run.Guid},
                {"area", ""},
                {_siteSettings.TaskExecutorKey, _siteSettings.TaskExecutorPassword}
            };
            string url = _urlHelper.Action("ExecuteNext", "BatchExecution", routeValues, "http");
            httpClient.GetAsync(url);
        }
    }
}