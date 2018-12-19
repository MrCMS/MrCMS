using Microsoft.AspNetCore.Mvc;

namespace MrCMS.Web.Apps.Admin.Infrastructure.Dashboard
{
    public abstract class DashboardViewComponent : ViewComponent
    {
        public abstract DashboardArea DashboardArea { get;  }

        public abstract int Order { get; }
    }
}