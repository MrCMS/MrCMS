using MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Apps.Admin.Breadcrumbs.System
{
    public class SitesBreadcrumb : Breadcrumb<SystemBreadcrumb>
    {
        public override int Order => 6;
        public override string Controller => "Sites";
        public override string Action => "Index";
        public override bool IsNav => true;
    }
}