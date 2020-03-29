using System.Threading.Tasks;
using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Areas.Admin.Breadcrumbs.System
{
    public class MessageBreadcrumb : Breadcrumb<MessageQueueBreadcrumb>
    {
        public override string Controller => "MessageQueue";
        public override string Action => "Show";
        public override Task Populate()
        {
            if (Id.HasValue)
            {
                Name = $"Message #{Id}";
            }

            return Task.CompletedTask;
        }
    }
}