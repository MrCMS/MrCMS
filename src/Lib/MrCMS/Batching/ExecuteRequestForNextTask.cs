using System.Net.Http;
using System.Threading.Tasks;
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

        public Task Execute(BatchRun run)
        {
            var httpClient = new HttpClient();
            var routeValues = new RouteValueDictionary
            {
                {"id", run.Guid},
                {"area", ""},
                {_siteSettings.TaskExecutorKey, _siteSettings.TaskExecutorPassword}
            };
            var url = _urlHelper.Action("ExecuteNext", "BatchExecution", routeValues, _urlHelper.ActionContext.HttpContext.Request.Scheme);
            httpClient.GetAsync(url);
            return Task.CompletedTask;
        }
    }
}