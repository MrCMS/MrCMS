using System.Web.Mvc;

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
                new {controller = "BatchExecution", action = "ExecuteNext"});

            context.MapRoute(
                "Admin_default1",
                "Admin/{controller}/{action}/{id}",
                new {controller = "Home", action = "Index", id = UrlParameter.Optional}
                );
        }
    }
}