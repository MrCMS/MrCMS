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
        private readonly IUrlHelper _urlHelper;
        private readonly IConfigurationProvider _configurationProvider;

        public ExecuteRequestForNextTask(IUrlHelper urlHelper, IConfigurationProvider configurationProvider)
        {
            _urlHelper = urlHelper;
            _configurationProvider = configurationProvider;
        }

        public async Task Execute(BatchRun run)
        {
            var httpClient = new HttpClient();
            var siteSettings = await _configurationProvider.GetSiteSettings<SiteSettings>();
            var routeValues = new RouteValueDictionary
            {
                {"id", run.Guid},
                {"area", ""},
                {siteSettings.TaskExecutorKey, siteSettings.TaskExecutorPassword}
            };
            var url = _urlHelper.Action("ExecuteNext", "BatchExecution", routeValues, _urlHelper.ActionContext.HttpContext.Request.Scheme);
#pragma warning disable
            httpClient.GetAsync(url);
        }
    }
}