using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Apps;
using MrCMS.Helpers;
using MrCMS.Tests.Services.Events;
using MrCMS.Website.CMS;
using MrCMS.Website.Controllers;
using NHibernate.Cfg;
using Xunit;

namespace MrCMS.Tests.Helpers
{
    public class TypeHelperTests
    {
        public TypeHelperTests()
        {
            TypeHelper.Initialize(GetType().Assembly);
        }

        [Fact]
        public void GenericTypeAssignableFromForInterfaces()
        {
            typeof(GenericInterface<>).IsGenericTypeDefinition.Should().BeTrue();

            var types = TypeHelper.GetAllConcreteTypesAssignableFrom(typeof(GenericInterface<>));

            types.Should().Contain(typeof(ImplementationOfGenericInterface));
        }

        [Fact]
        public void GenericTypeAssignableFromLogicConfirmed()
        {
            typeof(MrCMSAppAdminController<>).IsGenericTypeDefinition.Should().BeTrue();
            typeof(MrCMSAppAdminController<TestApp>).IsGenericTypeDefinition.Should().BeFalse();

            typeof(TestAdminController).GetBaseTypes()
                .Any(
                    type =>
                        type.IsGenericType &&
                        type.GetGenericTypeDefinition() == typeof(MrCMSAppAdminController<>))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void GenericTypeLoadingTest()
        {
            var types = typeof(TestAdminController).GetBaseTypes(type =>
                type.IsGenericType && type.GetGenericTypeDefinition() == typeof(MrCMSAppAdminController<>));

            types.FirstOrDefault().Should().Be(typeof(MrCMSAppAdminController<TestApp>));
        }

        [Fact]
        public void TypeHelper_GetAllConcreteTypesAssignableFrom_GenericTypePassedShouldReturnImplementations()
        {
            var types = TypeHelper.GetAllConcreteTypesAssignableFrom(typeof(MrCMSAppAdminController<>));

            types.Should().Contain(typeof(TestAdminController));
        }

        [Fact]
        public void TypeHelper_GetAllConcreteTypesAssignableFrom_GetsTypesFromAGivenInterface()
        {
            var types = TypeHelper.GetAllConcreteTypesAssignableFrom<ITestEvent>();

            types.Should().Contain(typeof(TestEventImplementation));
            types.Should().Contain(typeof(TestEventImplementation2));
        }
    }

    public interface GenericInterface<T>
    {
        T Test { get; }
    }

    public class ImplementationOfGenericInterface : GenericInterface<string>
    {
        public string Test => "Test";
    }


    public class TestAdminController : MrCMSAppAdminController<TestApp>
    {
    }

    public class TestApp : IMrCMSApp
    {
        public string Name => "Test";

        public string Version => "0.1";

        public string ContentPrefix { get; set; }
        public string ViewPrefix { get; set; }
        public Assembly Assembly { get; }
        public IEnumerable<Type> Conventions { get; }
        public IEnumerable<Type> BaseTypes { get; }
        public IDictionary<Type, string> SignalRHubs { get; }
        public IEnumerable<RegistrationInfo> Registrations { get; }

        public IServiceCollection RegisterServices(IServiceCollection serviceCollection)
        {
            return serviceCollection;
        }

        public IRouteBuilder MapRoutes(IRouteBuilder routeBuilder)
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