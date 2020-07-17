using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Admin.Breadcrumbs.System
{
    public class NotificationsBreadcrumb : Breadcrumb<SystemBreadcrumb>
    {
        public override int Order => 13;
        public override string Controller => "Notification";
        public override string Action => "Index";
        public override bool IsNav => true;
    }
}