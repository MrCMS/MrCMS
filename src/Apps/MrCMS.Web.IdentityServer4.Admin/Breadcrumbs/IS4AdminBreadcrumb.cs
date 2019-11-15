using MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Apps.IdentityServer4.Admin.Breadcrumbs
{
    public class IS4AdminBreadcrumb : Breadcrumb
    {
        public override int Order => 8;
        public override string Controller => "";
        public override string Action => "";
        public override bool IsPlaceHolder => true;
        public override bool IsNav => true;
        public override string CssClass => "fa fa-user-secret";

        public override string Name => "Identity Server";
    }
}