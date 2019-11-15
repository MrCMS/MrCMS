using MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Apps.IdentityServer4.Admin.Breadcrumbs.IS4Admin
{
    public class ClientsBreadcrumb : Breadcrumb<IS4AdminBreadcrumb>
    {
        public override int Order => 2;
        public override string Controller => "Configuration";
        public override string Action => "Clients";
        public override bool IsNav => true;

       
    }
}