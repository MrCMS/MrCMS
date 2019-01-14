using MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Apps.Admin.Breadcrumbs.System
{
    public class AboutBreadcrumb : Breadcrumb<SystemBreadcrumb>
    {
        public override int Order => 15;
        public override string Controller => "About";
        public override string Action => "Index";
        public override bool IsNav => true;
    }
}