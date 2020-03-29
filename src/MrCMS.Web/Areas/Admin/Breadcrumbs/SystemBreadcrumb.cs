using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Areas.Admin.Breadcrumbs
{
    public class SystemBreadcrumb : Breadcrumb
    {
        public override int Order => 100;
        public override string Controller => "";
        public override string Action => "";
        public override bool IsPlaceHolder => true;
        public override bool IsNav => true;
        public override string CssClass => "fa fa-cogs";
    }
}