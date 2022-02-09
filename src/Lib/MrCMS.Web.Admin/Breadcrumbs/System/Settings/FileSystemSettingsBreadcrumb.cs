using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Admin.Breadcrumbs.System.Settings
{
    public class FileSystemSettingsBreadcrumb : Breadcrumb<SettingsBreadcrumb>
    {
        public override string Controller => "Settings";
        public override string Action => "FileSystem";

        public override bool IsNav => true;
        public override decimal Order => 2;
    }
}