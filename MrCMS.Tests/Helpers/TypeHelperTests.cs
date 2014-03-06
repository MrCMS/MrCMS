using System;
using System.Collections.Generic;
using FluentAssertions;
using MrCMS.Apps;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Installation;
using MrCMS.Models;
using MrCMS.Website.Controllers;
using NHibernate;
using Ninject;
using Xunit;
using System.Linq;

namespace MrCMS.Tests.Helpers
{
    public class TypeHelperTests
    {
        [Fact]
        public void TypeHelper_GetAllConcreteTypesAssignableFrom_GetsTypesFromAGivenInterface()
        {
            var types = TypeHelper.GetAllConcreteTypesAssignableFrom<IAdminMenuItem>();

            types.Should().Contain(typeof(TestAdminMenuItem));
        }

        [Fact]
        public void TypeHelper_GetAllConcreteTypesAssignableFrom_GenericTypePassedShouldReturnImplementations()
        {
            var types = TypeHelper.GetAllConcreteTypesAssignableFrom(typeof(MrCMSAppAdminController<>));

            types.Should().Contain(typeof(TestAdminController));
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
        public void GenericTypeAssignableFromForInterfaces()
        {
            typeof(GenericInterface<>).IsGenericTypeDefinition.Should().BeTrue();

            var types = TypeHelper.GetAllConcreteTypesAssignableFrom(typeof(GenericInterface<>));
            
            types.Should().Contain(typeof (ImplementationOfGenericInterface));
            //typeof(ImplementationOfGenericInterface).GetBaseTypes()
            //                            .Any(
            //                                type =>
            //                                type.IsGenericType &&
            //                                type.GetGenericTypeDefinition() == typeof(GenericInterface<>))
            //                            .Should()
            //                            .BeTrue();
        }

        [Fact]
        public void GenericTypeLoadingTest()
        {
            var types = typeof (TestAdminController).GetBaseTypes(type => type.IsGenericType && type.GetGenericTypeDefinition() == typeof (MrCMSAppAdminController<>));

            types.FirstOrDefault().Should().Be(typeof (MrCMSAppAdminController<TestApp>));
        }


        private class TestAdminMenuItem : IAdminMenuItem
        {
            public string Text { get; private set; }
            public string Url { get; private set; }
            public bool CanShow { get; private set; }
            public IDictionary<string, List<IMenuItem>> Children { get; private set; }
            public int DisplayOrder { get; private set; }
        }
    }

    public interface GenericInterface<T>
    {
        T Test { get; }
    }
    public class ImplementationOfGenericInterface : GenericInterface<string>
    {
        public string Test { get { return "Test"; } }
    }


    public class TestAdminController : MrCMSAppAdminController<TestApp>
    {

    }

    public class TestApp : MrCMSApp
    {
        protected override void RegisterApp(MrCMSAppRegistrationContext context)
        {

        }

        public override string AppName
        {
            get { return "Test"; }
        }

        public override string Version
        {
            get { return "0.1"; }
        }

        protected override void RegisterServices(IKernel kernel)
        {
            
        }

        protected override void OnInstallation(ISession session, InstallModel model, Site site)
        {
            throw new NotImplementedException();
        }
    }
}