using MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Apps.Admin.Breadcrumbs
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