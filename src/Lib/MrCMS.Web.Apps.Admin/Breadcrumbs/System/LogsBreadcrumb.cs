using MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Apps.Admin.Breadcrumbs.System
{
    public class LogsBreadcrumb : Breadcrumb<SystemBreadcrumb>
    {
        public override int Order => 8;
        public override string Controller => "Log";
        public override string Action => "Index";
        public override bool IsNav => true;
    }
}