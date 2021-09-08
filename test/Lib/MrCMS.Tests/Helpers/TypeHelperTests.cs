using System.Linq;
using FluentAssertions;
using MrCMS.Helpers;
using MrCMS.Tests.Services.Events;
using MrCMS.Web.Admin.Infrastructure.BaseControllers;
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
        public void GenericTypeAssignableFromGenericForInterfaces()
        {
            typeof(GenericInterface<>).IsGenericTypeDefinition.Should().BeTrue();

            var types = TypeHelper.GetAllConcreteTypesAssignableFromGeneric(typeof(GenericInterface<>));

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
            var types = TypeHelper.GetAllConcreteTypesAssignableFromGeneric(typeof(MrCMSAppAdminController<>));

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
}