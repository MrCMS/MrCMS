using MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Apps.Admin.Breadcrumbs
{
    public class FormsBreadcrumb : Breadcrumb
    {
        public override int Order => 3;
        public override string Controller => "Form";
        public override string Action => "Index";
        public override bool IsNav => true;
        public override string CssClass => "fa fa-wpforms";
    }
}