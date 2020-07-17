using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Admin.Breadcrumbs.System
{
    public class IndexesBreadcrumb : Breadcrumb<SystemBreadcrumb>
    {
        public override int Order => 11;
        public override string Controller => "Indexes";
        public override string Action => "Index";
        public override bool IsNav => true;
    }
}