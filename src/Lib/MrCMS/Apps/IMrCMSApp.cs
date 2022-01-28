using System;
using System.Collections.Generic;
using System.Reflection;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Website.CMS;
using NHibernate.Cfg;

namespace MrCMS.Apps
{
    public interface IMrCMSApp
    {
        string Name { get; }
        string Version { get; }

        Assembly Assembly { get; }
        IEnumerable<Type> Conventions { get; }
        IEnumerable<Type> BaseTypes { get; }
        IDictionary<Type, string> SignalRHubs { get; }
        IEnumerable<ApplicationRegistrationInfo> Registrations { get; }
        IEnumerable<EndpointRegistrationInfo> EndpointRegistrations { get; }

        IServiceCollection RegisterServices(IServiceCollection serviceCollection, IConfiguration configuration);

        IEndpointRouteBuilder MapRoutes(IEndpointRouteBuilder routeBuilder);
        void SetupMvcOptions(MvcOptions options);
        void ConfigureAutomapper(IMapperConfigurationExpression expression);
        void AppendConfiguration(Configuration configuration);
        void ConfigureAuthorization(AuthorizationOptions options);
    }
}