using MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Apps.Admin.Breadcrumbs
{
    public class WebpagesBreadcrumb : Breadcrumb
    {
        public override string Controller => "Webpage";
        public override string Action => "Index";
        public override bool IsNav => true;
        public override string CssClass => "fa fa-file-code-o";
    }
}