using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Admin.Breadcrumbs.System.Security
{
    public class CustomScriptsBreadcrumb : Breadcrumb<SecurityBreadcrumb>
    {
        public override string Controller => "CustomScriptPages";
        public override string Action => "Index";
        public override bool IsNav => true;
        public override decimal Order => 0;
    }
}