using MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Apps.Admin.Breadcrumbs.System.Settings
{
    public class SiteSettingsBreadcrumb : Breadcrumb<SettingsBreadcrumb>
    {
        public override string Name => "Site Settings";
        public override string Controller => "Settings";
        public override string Action => "Index";

        public override bool IsNav => true;
        public override int Order => 0;
    }
}