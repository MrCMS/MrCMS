using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using MrCMS.Batching.Entities;
using MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Apps.Admin.Breadcrumbs.System
{
    public class BatchRunBreadcrumb : Breadcrumb<BatchRunsBreadcrumb>
    {
        public override string Controller => "BatchRun";
        public override string Action => "Show";

        public override Task Populate()
        {
            if (!Id.HasValue)
            {
                return Task.CompletedTask;
            }

            Name = $"Run #{Id}";
            if (ActionArguments["batchRun"] is BatchRun batchRun)
            {
                ParentActionArguments = new RouteValueDictionary { ["batch"] = batchRun.Batch };
            }
            return Task.CompletedTask;
        }
    }
}