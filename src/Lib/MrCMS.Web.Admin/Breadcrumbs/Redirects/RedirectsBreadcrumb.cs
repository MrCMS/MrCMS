using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Admin.Breadcrumbs.Redirects
{
    public class RedirectsBreadcrumb : Breadcrumb
    {
        public override decimal Order => 20;
        public override string Controller => "";
        public override string Action => "";
        public override bool IsNav => true;
        public override bool IsPlaceHolder => true;
        public override string CssClass => "fa fa-reply";
    }
}