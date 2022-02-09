using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Admin.Breadcrumbs
{
    public class FormsBreadcrumb : Breadcrumb
    {
        public override decimal Order => 3;
        public override string Controller => "Form";
        public override string Action => "Index";
        public override bool IsNav => true;
        public override string CssClass => "fa fa-wpforms";
    }
}