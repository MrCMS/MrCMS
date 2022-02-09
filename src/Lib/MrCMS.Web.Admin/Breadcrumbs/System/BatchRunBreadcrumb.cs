using Microsoft.AspNetCore.Routing;
using MrCMS.Batching.Entities;
using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;
using NHibernate;

namespace MrCMS.Web.Admin.Breadcrumbs.System
{
    public class BatchRunBreadcrumb : Breadcrumb<BatchRunsBreadcrumb>
    {
        private readonly ISession _session;

        public BatchRunBreadcrumb(ISession session)
        {
            _session = session;
        }

        public override string Controller => "BatchRun";
        public override string Action => "Show";

        public override void Populate()
        {
            if (!Id.HasValue)
            {
                return;
            }

            Name = $"Run #{Id}";
            var batchRun = _session.Get<BatchRun>(Id.Value);
            if (batchRun != null)
                ParentActionArguments = new RouteValueDictionary {["id"] = batchRun.Batch.Id};
        }
    }
}