using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Admin.Breadcrumbs.System
{
    public class MessageQueueBreadcrumb : Breadcrumb<SystemBreadcrumb>
    {
        public override decimal Order => 12;
        public override string Controller => "MessageQueue";
        public override string Action => "Index";
        public override bool IsNav => true;
    }
}