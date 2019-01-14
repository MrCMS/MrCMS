using MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Apps.Admin.Breadcrumbs.System
{
    public class IndexesBreadcrumb : Breadcrumb<SystemBreadcrumb>
    {
        public override int Order => 11;
        public override string Controller => "Indexes";
        public override string Action => "Index";
        public override bool IsNav => true;
    }
}