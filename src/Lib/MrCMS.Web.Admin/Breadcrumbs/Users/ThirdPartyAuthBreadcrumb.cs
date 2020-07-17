using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Admin.Breadcrumbs.Users
{
    public class ThirdPartyAuthBreadcrumb : Breadcrumb<UsersBreadcrumb>
    {
        public override int Order => 2;
        public override string Controller => "ThirdPartyAuth";
        public override string Action => "Index";
        public override bool IsNav => true;
    }
}