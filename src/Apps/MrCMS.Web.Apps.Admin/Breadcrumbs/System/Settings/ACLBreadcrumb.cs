using MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Apps.Admin.Breadcrumbs.System.Settings
{
    public class ACLBreadcrumb : Breadcrumb<SettingsBreadcrumb>
    {
        public override string Name => "ACL";
        public override string Controller => "ACL";
        public override string Action => "Index";

        public override bool IsNav => true;
        public override int Order => 4;
    }
}