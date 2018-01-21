using FakeItEasy;
using FluentAssertions;
using MrCMS.Website;
using Ninject;
using Xunit;

namespace MrCMS.Tests.IoC
{
    public class NinjectScopingTests
    {
        private readonly StandardKernel _kernel;

        public NinjectScopingTests()
        {
            _kernel = new StandardKernel();
            MrCMSKernel.OverrideKernel(_kernel);
            _kernel.Bind<ITestInterface>().ToMethod(context => A.Fake<ITestInterface>()).InThreadScope();
        }

        [Fact]
        public void Kernel_Get_RequestingTheSameItemTwiceShouldGetTheSameInstanceTwice()
        {
            var documentService1 = _kernel.Get<ITestInterface>();
            var documentService2 = _kernel.Get<ITestInterface>();

            documentService1.Should().BeSameAs(documentService2);
        }

        public interface ITestInterface
        {
            
        }
    }
    
}