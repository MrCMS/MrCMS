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
using MrCMS.Helpers;

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
        public virtual IEnumerable<RegistrationInfo> Registrations => Enumerable.Empty<RegistrationInfo>();

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

        public virtual void ConfigureAutomapper(IMapperConfigurationExpression expression)
        {
            expression.AddProfiles(Assembly.GetTypes().Where(x => x.IsImplementationOf(typeof(Profile)))
                .Select(Activator.CreateInstance).Cast<Profile>());
        }

        public virtual void AppendConfiguration(Configuration configuration) { }
        public virtual void ConfigureAuthorization(AuthorizationOptions options) { }
    }
}