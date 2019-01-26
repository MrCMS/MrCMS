using MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Apps.Admin.Breadcrumbs.System
{
    public class AddSiteBreadcrumb : Breadcrumb<SitesBreadcrumb>
    {
        public override string Controller => "Sites";
        public override string Action => "Add";
        public override string Name => "Add";
    }
}