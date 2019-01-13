using MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Apps.Admin.Breadcrumbs.System
{
    public class MessageBreadcrumb : Breadcrumb<MessageQueueBreadcrumb>
    {
        public override string Controller => "MessageQueue";
        public override string Action => "Show";
        public override void Populate()
        {
            if (Id.HasValue)
            {
                Name = $"Message #{Id}";
            }
        }
    }
}