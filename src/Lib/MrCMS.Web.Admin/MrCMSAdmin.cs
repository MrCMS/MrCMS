using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MrCMS.Apps;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Web.Admin.Filters;
using MrCMS.Web.Admin.Hubs;
using MrCMS.Web.Admin.ModelBinders;
using MrCMS.Web.Admin.Helpers;
using MrCMS.Website.Profiling;

namespace MrCMS.Web.Admin
{
    public class MrCMSAdmin : StandardMrCMSApp
    {
        public MrCMSAdmin()
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
            options.Filters.AddService<ProfilingAuthorizationFilter<AdminAuthFilter>>();
            options.Filters.AddService<ProfilingAsyncActionFilter<BreadcrumbActionFilter>>();
        }

        public override IDictionary<Type, string> SignalRHubs { get; } = new Dictionary<Type, string>
        {
            [typeof(BatchProcessingHub)] = "/batchHub",
            [typeof(NotificationHub)] = "/notificationsHub",
        };
    }
}