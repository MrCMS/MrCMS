using System;
using System.Reflection;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Apps;

namespace MrCMS.Web.Apps.Admin
{
    public class MrCMSAdminApp : IMrCMSApp
    {
        public string ContentPrefix { get; set; } = "/Areas/Admin";
        public Assembly Assembly => GetType().Assembly;
        public IServiceCollection RegisterServices(IServiceCollection serviceCollection)
        {
            return serviceCollection;
        }

        public IRouteBuilder MapRoutes(IRouteBuilder routeBuilder)
        {
            return routeBuilder;
        }
    }
}
