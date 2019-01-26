using MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Apps.Admin.Breadcrumbs.System
{
    public class BatchBreadcrumb : Breadcrumb<BatchesBreadcrumb>
    {
        public override string Controller => "Batch";
        public override string Action => "Show";
        public override void Populate()
        {
            if (Id.HasValue)
            {
                Name = $"Batch #{Id}";
            }
        }
    }
}