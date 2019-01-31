using MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Apps.Admin.Breadcrumbs.System.Settings
{
    public class MailSettingsBreadcrumb : Breadcrumb<SettingsBreadcrumb>
    {
        public override string Controller => "SystemSettings";
        public override string Action => "Mail";

        public override bool IsNav => true;
        public override int Order => 3;
    }
}