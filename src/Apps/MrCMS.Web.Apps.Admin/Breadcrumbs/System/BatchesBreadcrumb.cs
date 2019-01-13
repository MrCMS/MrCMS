using MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Apps.Admin.Breadcrumbs.System
{
    public class BatchesBreadcrumb : Breadcrumb<SystemBreadcrumb>
    {
        public override int Order => 9;
        public override string Controller => "Batch";
        public override string Action => "Index";
        public override bool IsNav => true;
    }
}