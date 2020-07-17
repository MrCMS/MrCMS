using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Admin.Breadcrumbs.System.Security
{
    public class SecurityOptionsBreadcrumb : Breadcrumb<SecurityBreadcrumb>
    {
        public override string Controller => "SecurityOptions";
        public override string Action => "Index";
        public override bool IsNav => true;
        public override int Order => 1;
    }
}