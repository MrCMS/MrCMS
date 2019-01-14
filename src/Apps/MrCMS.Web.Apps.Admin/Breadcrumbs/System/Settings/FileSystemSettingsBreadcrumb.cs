using MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Apps.Admin.Breadcrumbs.System.Settings
{
    public class FileSystemSettingsBreadcrumb : Breadcrumb<SettingsBreadcrumb>
    {
        public override string Controller => "Settings";
        public override string Action => "FileSystem";

        public override bool IsNav => true;
        public override int Order => 2;
    }
}