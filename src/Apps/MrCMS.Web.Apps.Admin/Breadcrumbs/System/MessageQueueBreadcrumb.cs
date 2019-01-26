using MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Apps.Admin.Breadcrumbs.System
{
    public class MessageQueueBreadcrumb : Breadcrumb<SystemBreadcrumb>
    {
        public override int Order => 12;
        public override string Controller => "MessageQueue";
        public override string Action => "Index";
        public override bool IsNav => true;
    }
}