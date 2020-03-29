using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Areas.Admin.Breadcrumbs.System
{
    public class AddSiteBreadcrumb : Breadcrumb<SitesBreadcrumb>
    {
        public override string Controller => "Sites";
        public override string Action => "Add";
        public override string Name => "Add";
    }
}