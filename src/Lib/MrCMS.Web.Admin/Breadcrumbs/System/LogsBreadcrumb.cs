using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Admin.Breadcrumbs.System
{
    public class LogsBreadcrumb : Breadcrumb<SystemBreadcrumb>
    {
        public override decimal Order => 8;
        public override string Controller => "Log";
        public override string Action => "Index";
        public override bool IsNav => true;
    }
}