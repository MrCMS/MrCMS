using System.Reflection;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace MrCMS.Apps
{
    public abstract class StandardMrCMSApp : IMrCMSApp
    {
        public abstract string Name { get; }
        public virtual Assembly Assembly => GetType().Assembly;
        public string ContentPrefix { get; set; }
        public string ViewPrefix { get; set; }


        public virtual IServiceCollection RegisterServices(IServiceCollection serviceCollection)
        {
            return serviceCollection;
        }

        public virtual IRouteBuilder MapRoutes(IRouteBuilder routeBuilder)
        {
            return routeBuilder;
        }

        public virtual void SetupMvcOptions(MvcOptions options) { }

        public virtual void ConfigureAutomapper(IMapperConfigurationExpression expression) => expression.AddProfiles(Assembly);
    }
}