using Microsoft.AspNetCore.Routing;
using MrCMS.Batching.Entities;
using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Admin.Breadcrumbs.System
{
    public class BatchRunBreadcrumb : Breadcrumb<BatchRunsBreadcrumb>
    {
        public override string Controller => "BatchRun";
        public override string Action => "Show";

        public override void Populate()
        {
            if (!Id.HasValue)
            {
                return;
            }

            Name = $"Run #{Id}";
            if (ActionArguments["batchRun"] is BatchRun batchRun)
            {
                ParentActionArguments = new RouteValueDictionary { ["batch"] = batchRun.Batch };
            }
        }
    }
}