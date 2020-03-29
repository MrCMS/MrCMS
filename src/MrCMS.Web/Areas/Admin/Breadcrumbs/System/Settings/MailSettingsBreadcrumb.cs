using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Areas.Admin.Breadcrumbs.System.Settings
{
    public class MailSettingsBreadcrumb : Breadcrumb<SettingsBreadcrumb>
    {
        public override string Controller => "SystemSettings";
        public override string Action => "Mail";

        public override bool IsNav => true;
        public override int Order => 3;
    }
}