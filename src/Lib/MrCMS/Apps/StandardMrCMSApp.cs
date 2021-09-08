using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Website.CMS;
using NHibernate.Cfg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;

namespace MrCMS.Apps
{
    public abstract class StandardMrCMSApp : IMrCMSApp
    {
        public abstract string Name { get; }
        public abstract string Version { get; }
        public virtual Assembly Assembly => GetType().Assembly;
        public virtual IEnumerable<Type> Conventions => Enumerable.Empty<Type>();

        public virtual IEnumerable<Type> BaseTypes => Enumerable.Empty<Type>();
        public virtual IDictionary<Type, string> SignalRHubs => new Dictionary<Type, string>();

        public virtual IEnumerable<ApplicationRegistrationInfo> Registrations =>
            Enumerable.Empty<ApplicationRegistrationInfo>();

        public virtual IEnumerable<EndpointRegistrationInfo> EndpointRegistrations =>
            Enumerable.Empty<EndpointRegistrationInfo>();


        public virtual IServiceCollection RegisterServices(IServiceCollection serviceCollection)
        {
            return serviceCollection;
        }

        public virtual IEndpointRouteBuilder MapRoutes(IEndpointRouteBuilder routeBuilder)
        {
            return routeBuilder;
        }

        public virtual void SetupMvcOptions(MvcOptions options)
        {
        }

        public virtual void ConfigureAutomapper(IMapperConfigurationExpression expression)
        {
            expression.AddProfiles(Assembly.GetTypes().Where(x => typeof(Profile).IsAssignableFrom(x))
                .Select(Activator.CreateInstance).Cast<Profile>());
        }

        public virtual void AppendConfiguration(Configuration configuration)
        {
        }

        public virtual void ConfigureAuthorization(AuthorizationOptions options)
        {
        }
    }
}