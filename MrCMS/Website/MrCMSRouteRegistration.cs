using System.Web.Mvc;
using System.Web.Routing;
using MrCMS.Batching;
using MrCMS.Tasks;
using MrCMS.Website.Controllers;
using MrCMS.Website.Routing;

namespace MrCMS.Website
{
    public static class MrCMSRouteRegistration
    {
        public static void Register(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("favicon.ico");

            routes.MapRoute("InstallerRoute", "install", new { controller = "Install", action = "Setup" });

            routes.MapRoute("Azure Probe Route", "azure-probe",
                new {controller = "AzureProbe", action = "KeepAlive"});

            routes.MapRoute("Task Execution", TaskExecutionController.ExecutePendingTasksURL,
                new { controller = "TaskExecution", action = "Execute" });
            routes.MapRoute("Individual Task Execution", TaskExecutionController.ExecuteTaskURL,
                new { controller = "TaskExecution", action = "ExecuteTask" });
            routes.MapRoute("Queued Task Execution", TaskExecutionController.ExecuteQueuedTasksURL,
                new { controller = "TaskExecution", action = "ExecuteQueuedTasks" });

            routes.MapRoute("batch execute", BatchExecutionController.BaseURL+"{id}",
                new {controller = "BatchExecution", action = "ExecuteNext"});

            routes.MapRoute("robots.txt", "robots.txt", new { controller = "SEO", action = "Robots" });
            routes.MapRoute("ckeditor Config", "Areas/Admin/Content/Editors/ckeditor/config.js",
                new { controller = "CKEditor", action = "Config" });

            routes.MapRoute("Logout", "logout", new { controller = "Logout", action = "Logout" },
                new[] { typeof(LogoutController).Namespace });

            routes.MapRoute("zones", "render-widget", new { controller = "Widget", action = "Show" },
                new[] { typeof(WidgetController).Namespace });

            routes.MapRoute("ajax content save", "admintools/savebodycontent",
                new { controller = "AdminTools", action = "SaveBodyContent" });

            routes.MapRoute("form save", "save-form/{id}", new { controller = "Form", action = "Save" },
                new[] { typeof(FormController).Namespace });

            routes.Add(new Route("{*data}", new RouteValueDictionary(), new RouteValueDictionary(),
                new MrCMSRouteHandler()));
        }
    }
}