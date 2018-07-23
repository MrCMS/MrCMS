using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MrCMS.Batching.Entities;
using MrCMS.Settings;

namespace MrCMS.Batching
{
    public class ExecuteRequestForNextTask : IExecuteRequestForNextTask
    {
        private readonly SiteSettings _siteSettings;
        private readonly IUrlHelper _urlHelper;

        public ExecuteRequestForNextTask(IUrlHelper urlHelper, SiteSettings siteSettings)
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
            var url = _urlHelper.Action("ExecuteNext", "BatchExecution", routeValues, "http");
            httpClient.GetAsync(url);
        }
    }
}