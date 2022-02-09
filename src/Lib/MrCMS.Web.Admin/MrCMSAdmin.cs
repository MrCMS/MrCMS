using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MrCMS.Apps;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Web.Admin.Filters;
using MrCMS.Web.Admin.Hubs;
using MrCMS.Web.Admin.ModelBinders;
using MrCMS.Web.Admin.Helpers;
using MrCMS.Website.CMS;

namespace MrCMS.Web.Admin
{
    public class MrCMSAdmin : StandardMrCMSApp
    {
        public MrCMSAdmin()
        {
            // ContentPrefix = "/Areas/Admin";
        }

        public override string Name => "Admin";
        public override string Version => "1.0";

        public override IEndpointRouteBuilder MapRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute("ckeditor Config", "Areas/Admin/Content/lib/ckeditor/config.js",
                new {controller = "CKEditor", action = "Config"});

            endpointRouteBuilder.MapAreaControllerRoute("Admin route",
                "Admin",
                "Admin/{controller}/{action}/{id?}",
                new {controller = "Home", action = "Index"}
            );

            return endpointRouteBuilder;
        }

        public override IServiceCollection RegisterServices(IServiceCollection serviceCollection,
            IConfiguration configuration)
        {
            serviceCollection.RegisterBreadcrumbs();
            return base.RegisterServices(serviceCollection, configuration);
        }

        public override void SetupMvcOptions(MvcOptions options)
        {
            options.ModelBinderProviders.Insert(1, new UpdateAdminViewModelBinderProvider());
            options.Filters.AddService<AdminAuthFilter>();
            options.Filters.AddService<BreadcrumbActionFilter>();
        }

        public override IDictionary<Type, string> SignalRHubs { get; } = new Dictionary<Type, string>
        {
            [typeof(BatchProcessingHub)] = "/batchHub",
            [typeof(NotificationHub)] = "/notificationsHub",
        };

        public override IEnumerable<EndpointRegistrationInfo> EndpointRegistrations
        {
            get
            {
                yield return new EndpointRegistrationInfo
                {
                    Order = 10000,
                    Registration = builder =>
                    {
                        builder.MapHub<NotificationHub>("/notificationsHub");
                        builder.MapHub<BatchProcessingHub>("/batchHub");
                    }
                };
            }
        }
    }
}