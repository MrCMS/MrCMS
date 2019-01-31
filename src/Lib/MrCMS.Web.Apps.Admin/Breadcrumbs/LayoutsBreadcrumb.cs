using MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Apps.Admin.Breadcrumbs
{
    public class LayoutsBreadcrumb : Breadcrumb
    {
        public override int Order => 2;
        public override string Controller => "Layout";
        public override string Action => "Index";
        public override bool IsNav => true;
        public override string CssClass => "fa fa-delicious";
    }
}