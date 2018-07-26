using System.Reflection;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace MrCMS.Apps
{
    public interface IMrCMSApp
    {
        string Name { get; }
        string ContentPrefix { get; set; }
        string ViewPrefix { get; set; }

        Assembly Assembly { get; }

        IServiceCollection RegisterServices(IServiceCollection serviceCollection);

        IRouteBuilder MapRoutes(IRouteBuilder routeBuilder);
    }
}