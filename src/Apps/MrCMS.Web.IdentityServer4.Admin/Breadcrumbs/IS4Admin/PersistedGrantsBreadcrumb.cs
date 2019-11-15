using MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Apps.IdentityServer4.Admin.Breadcrumbs.IS4Admin
{
    public class PersistedGrantsBreadcrumb : Breadcrumb<IS4AdminBreadcrumb>
    {
        public override int Order => 3;
        public override string Controller => "Grant";
        public override string Action => "PersistedGrants";
        public override bool IsNav => true;


    }
}