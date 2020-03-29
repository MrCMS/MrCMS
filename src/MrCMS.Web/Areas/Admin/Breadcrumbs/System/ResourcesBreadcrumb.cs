using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Areas.Admin.Breadcrumbs.System
{
    public class ResourcesBreadcrumb : Breadcrumb<SystemBreadcrumb>
    {
        public override int Order => 7;
        public override string Controller => "Resource";
        public override string Action => "Index";
        public override bool IsNav => true;
    }
}