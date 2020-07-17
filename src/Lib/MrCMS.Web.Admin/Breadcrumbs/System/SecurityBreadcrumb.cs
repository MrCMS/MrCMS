using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Admin.Breadcrumbs.System
{
    public class SecurityBreadcrumb : Breadcrumb<SystemBreadcrumb>
    {
        public override int Order => 1;
        public override string Controller => "";
        public override string Action => "";
        public override bool IsPlaceHolder => true;
        public override bool IsNav => true;
    }
}