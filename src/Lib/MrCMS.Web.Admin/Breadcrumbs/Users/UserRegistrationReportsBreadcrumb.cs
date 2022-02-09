using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Admin.Breadcrumbs.Users
{
    public class UserRegistrationReportsBreadcrumb : Breadcrumb<UsersBreadcrumb>
    {
        public override string Name => "Registrations Report";
        public override string Controller => "UserRegistrationReports";
        public override string Action => "Index";
        public override bool IsNav => true;
        public override decimal Order => 4;
    }
}