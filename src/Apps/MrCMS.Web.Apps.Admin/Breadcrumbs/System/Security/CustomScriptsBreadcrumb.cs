using MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Apps.Admin.Breadcrumbs.System.Security
{
    public class CustomScriptsBreadcrumb : Breadcrumb<SecurityBreadcrumb>
    {
        public override string Controller => "CustomScriptPages";
        public override string Action => "Index";
        public override bool IsNav => true;
        public override int Order => 0;
    }
}