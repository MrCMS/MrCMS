using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Areas.Admin.Breadcrumbs.Users
{
    public class UserSubscriptionReportsBreadcrumb : Breadcrumb<UsersBreadcrumb>
    {
        public override string Name => "User Subscription Reports";
        public override string Controller => "UserSubscriptionReports";
        public override string Action => "Index";
        public override bool IsNav => true;
        public override int Order => 4;
    }
}