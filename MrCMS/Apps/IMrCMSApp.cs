using System.Reflection;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace MrCMS.Apps
{
    public interface IMrCMSApp
    {
        Assembly Assembly { get; }

        IServiceCollection RegisterServices(IServiceCollection serviceCollection);

        IRouteBuilder MapRoutes(IRouteBuilder routeBuilder);
    }
}