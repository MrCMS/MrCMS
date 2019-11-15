using MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Apps.Admin.Breadcrumbs.Users
{
    public class ThirdPartyAuthBreadcrumb : Breadcrumb<UsersBreadcrumb>
    {
        public override int Order => 2;
        public override string Controller => "ThirdPartyAuth";
        public override string Action => "Index2";
        public override bool IsNav => true;
    }
}