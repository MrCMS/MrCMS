using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Areas.Admin.Breadcrumbs.System
{
    public class PageDefaultsBreadcrumb : Breadcrumb<SystemBreadcrumb>
    {
        public override int Order => 5;
        public override string Controller => "PageDefaults";
        public override string Action => "Index";
        public override bool IsNav => true;
    }
}