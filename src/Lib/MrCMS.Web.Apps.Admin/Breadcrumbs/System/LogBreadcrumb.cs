using System.Threading.Tasks;
using MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Apps.Admin.Breadcrumbs.System
{
    public class LogBreadcrumb : Breadcrumb<LogsBreadcrumb>
    {
        public override string Controller => "Log";
        public override string Action => "Show";
        public override Task Populate()
        {
            if (Id.HasValue)
            {
                Name = $"Log #{Id:N0}";
            }
            return Task.CompletedTask;
        }
    }
}