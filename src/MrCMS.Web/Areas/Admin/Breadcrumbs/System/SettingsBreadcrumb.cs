using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Areas.Admin.Breadcrumbs.System
{
    public class SettingsBreadcrumb : Breadcrumb<SystemBreadcrumb>
    {
        public override int Order => 0;
        public override string Controller => "";
        public override string Action => "";
        public override bool IsPlaceHolder => true;
        public override bool IsNav => true;
    }
}