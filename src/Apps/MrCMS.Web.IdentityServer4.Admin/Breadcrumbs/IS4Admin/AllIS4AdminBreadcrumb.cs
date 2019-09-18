using MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Apps.IdentityServer4.Admin.Breadcrumbs.IS4Admin
{
    public class DashBoardBreadcrumb : Breadcrumb<IS4AdminBreadcrumb>
    {
        public override int Order => 1;
        public override string Controller => "IS4AdminHome";
        public override string Action => "Index";
        public override bool IsNav => true;

        public override string Title => "DashBoard";

        public override string Name => "DashBoard";
    }
}