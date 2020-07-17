using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Admin.Breadcrumbs.System
{
    public class PageTemplatesBreadcrumb : Breadcrumb<SystemBreadcrumb>
    {
        public override int Order => 4;
        public override string Controller => "PageTemplate";
        public override string Action => "Index";
        public override bool IsNav => true;
    }
}