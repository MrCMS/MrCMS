using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Shortcodes.Forms;
using Ninject.MockingKernel;
using Xunit;

namespace MrCMS.Tests.Shortcodes.Forms
{
    public class ElementRendererManagerTests 
    {
        private readonly ElementRendererManager _elementRendererManager;
        private readonly MockingKernel _kernel = new MockingKernel();

        public ElementRendererManagerTests()
        {
            _elementRendererManager = new ElementRendererManager(_kernel);
        }

        [Fact]
        public void ElementRendererManager_GetElementRenderer_ShouldReturnTheResultOfTheTypeOfAPropertyFromTheKernel()
        {
            var formElementRenderer = A.Fake<IFormElementRenderer<TextBox>>();
            _kernel.Bind<IFormElementRenderer<TextBox>>()
                         .ToMethod(context => formElementRenderer)
                         .InSingletonScope();

            var renderer = _elementRendererManager.GetElementRenderer(new TextBox());

            renderer.Should().Be(formElementRenderer);
        }
    }
}