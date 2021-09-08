using System;
using System.Collections.Generic;
using System.Reflection;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Apps;
using MrCMS.Website.CMS;
using NHibernate.Cfg;

namespace MrCMS.Tests.Helpers
{
    public class TestApp : IMrCMSApp
    {
        public string Name => "Test";

        public string Version => "0.1";

        public string ContentPrefix { get; set; }
        public Assembly Assembly { get; }
        public IEnumerable<Type> Conventions { get; }
        public IEnumerable<Type> BaseTypes { get; }
        public IDictionary<Type, string> SignalRHubs { get; }
        public IEnumerable<ApplicationRegistrationInfo> Registrations { get; }
        public IEnumerable<EndpointRegistrationInfo> EndpointRegistrations { get; }

        public IServiceCollection RegisterServices(IServiceCollection serviceCollection)
        {
            return serviceCollection;
        }

        public IEndpointRouteBuilder MapRoutes(IEndpointRouteBuilder routeBuilder)
        {
            return routeBuilder;
        }

        public void SetupMvcOptions(MvcOptions options)
        {
        }

        public void ConfigureAutomapper(IMapperConfigurationExpression expression)
        {
        }

        public void AppendConfiguration(Configuration configuration)
        {
        }

        public void ConfigureAuthorization(AuthorizationOptions options)
        {
        }
    }
}