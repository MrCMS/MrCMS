using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Admin.Breadcrumbs.System
{
    public class TextSearchBreadcrumb : Breadcrumb<SystemBreadcrumb>
    {
        public override decimal Order => 11;
        public override string Controller => "TextSearch";
        public override string Action => "Index";
        public override bool IsNav => true;
    }
}