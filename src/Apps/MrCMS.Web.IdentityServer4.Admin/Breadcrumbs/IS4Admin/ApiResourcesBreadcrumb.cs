using MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Apps.IdentityServer4.Admin.Breadcrumbs.IS4Admin
{
    public class ApiResourcesBreadcrumb : Breadcrumb<IS4AdminBreadcrumb>
    {
        public override int Order => 3;
        public override string Controller => "Configuration";
        public override string Action => "ApiResources";
        public override bool IsNav => true;


    }
}