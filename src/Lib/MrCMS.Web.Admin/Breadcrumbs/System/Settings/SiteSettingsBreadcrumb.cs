using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Admin.Breadcrumbs.System.Settings
{
    public class SiteSettingsBreadcrumb : Breadcrumb<SettingsBreadcrumb>
    {
        public override string Name => "Site Settings";
        public override string Controller => "Settings";
        public override string Action => "Index";

        public override bool IsNav => true;
        public override decimal Order => 0;
    }
}