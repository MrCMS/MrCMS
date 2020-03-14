using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MrCMS.Apps;
using MrCMS.Web.Apps.Admin.Filters;
using MrCMS.Web.Apps.Admin.Hubs;
using MrCMS.Web.Apps.Admin.ModelBinders;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Web.Apps.Admin.Helpers;
using MrCMS.Website.Profiling;

namespace MrCMS.Web.Apps.Admin
{
    public class MrCMSAdminApp : StandardMrCMSApp
    {
        public MrCMSAdminApp()
        {
            ContentPrefix = "/Areas/Admin";
            ViewPrefix = "/Areas/Admin";
        }

        public override string Name => "Admin";
        public override string Version => "1.0";

        public override IRouteBuilder MapRoutes(IRouteBuilder routeBuilder)
        {
            routeBuilder.MapRoute("ckeditor Config", "Areas/Admin/lib/ckeditor/config.js",
                new { controller = "CKEditor", action = "Config" });

            routeBuilder.MapAreaRoute("Admin route",
                "Admin",
                "Admin/{controller}/{action}/{id?}",
                new { controller = "Home", action = "Index" }
            );

            return routeBuilder;
        }

        public override IServiceCollection RegisterServices(IServiceCollection serviceCollection)
        {
            serviceCollection.RegisterBreadcrumbs();
            return base.RegisterServices(serviceCollection);
        }

        public override void SetupMvcOptions(MvcOptions options)
        {
            options.ModelBinderProviders.Insert(1, new UpdateAdminViewModelBinderProvider());
            options.Filters.Add<ProfilingAsyncAuthorizationFilter<AdminAuthFilter>>();
            options.Filters.Add<ProfilingAsyncActionFilter<BreadcrumbActionFilter>>();
        }

        public override IDictionary<Type, string> SignalRHubs { get; } = new Dictionary<Type, string>
        {
            [typeof(BatchProcessingHub)] = "/batchHub",
            [typeof(NotificationHub)] = "/notificationsHub",
        };
    }
}