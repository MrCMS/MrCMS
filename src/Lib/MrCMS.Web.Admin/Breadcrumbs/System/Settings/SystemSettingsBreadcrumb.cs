using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Admin.Breadcrumbs.System.Settings
{
    public class SystemSettingsBreadcrumb : Breadcrumb<SettingsBreadcrumb>
    {
        public override string Controller => "SystemSettings";
        public override string Action => "Index";

        public override bool IsNav => true;
        public override int Order => 1;
    }
}