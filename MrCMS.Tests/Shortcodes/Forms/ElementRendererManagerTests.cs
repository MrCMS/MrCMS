using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Shortcodes.Forms;
using Xunit;

namespace MrCMS.Tests.Shortcodes.Forms
{
    public class ElementRendererManagerTests :MrCMSTest
    {
        private readonly ElementRendererManager _elementRendererManager;

        public ElementRendererManagerTests()
        {
            _elementRendererManager = new ElementRendererManager(Kernel);
        }

        [Fact]
        public void ElementRendererManager_GetElementRenderer_ShouldReturnTheResultOfTheTypeOfAPropertyFromTheKernel()
        {
            var formElementRenderer = A.Fake<IFormElementRenderer<TextBox>>();
            Kernel.Bind<IFormElementRenderer<TextBox>>()
                         .ToMethod(context => formElementRenderer)
                         .InSingletonScope();

            var renderer = _elementRendererManager.GetElementRenderer(new TextBox());

            renderer.Should().Be(formElementRenderer);
        }
    }
}