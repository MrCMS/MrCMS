using FakeItEasy;
using FluentAssertions;
using MrCMS.Services;
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
            MrCMSApplication.OverrideKernel(_kernel);
            _kernel.Bind<IDocumentService>().ToMethod(context => A.Fake<IDocumentService>()).InThreadScope();
        }

        [Fact]
        public void Kernel_Get_RequestingTheSameItemTwiceShouldGetTheSameInstanceTwice()
        {
            var documentService1 = _kernel.Get<IDocumentService>();
            var documentService2 = _kernel.Get<IDocumentService>();

            documentService1.Should().BeSameAs(documentService2);
        }
    }
}