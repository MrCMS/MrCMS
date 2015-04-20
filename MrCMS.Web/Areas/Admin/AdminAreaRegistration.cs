using System.Linq;
using System.Web.Mvc;
using System.Web.Optimization;
using MrCMS.Apps;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Website;
using Ninject;

namespace MrCMS.Web.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get { return "Admin"; }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute("batch execute", "batch-run/next/{id}",
                new { controller = "BatchExecution", action = "ExecuteNext" });

            context.MapRoute(
                "Admin_default1",
                "Admin/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }
                );
        }
    }
}