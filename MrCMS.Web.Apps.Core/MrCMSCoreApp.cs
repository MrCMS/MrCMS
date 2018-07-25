using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Apps;

namespace MrCMS.Web.Apps.Core
{
    public class MrCMSCoreApp : IMrCMSApp
    {
        public string ContentPrefix { get; set; } = "/Apps/Core";

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