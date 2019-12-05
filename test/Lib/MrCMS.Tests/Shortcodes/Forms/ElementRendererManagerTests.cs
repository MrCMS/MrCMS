using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Helpers;
using MrCMS.Shortcodes.Forms;
using Xunit;

namespace MrCMS.Tests.Shortcodes.Forms
{
    public class ElementRendererManagerTests 
    {
        private readonly IServiceCollection _serviceCollection = new ServiceCollection();

        public ElementRendererManagerTests()
        {
            TypeHelper.Initialize(GetType().Assembly);
        }

        public ElementRendererManager GetSUT()
        {
            return new ElementRendererManager(_serviceCollection.BuildServiceProvider());
        }

        [Fact]
        public void ElementRendererManager_GetElementRenderer_ShouldReturnTheResultOfTheTypeOfAPropertyFromTheKernel()
        {
            var formElementRenderer = A.Fake<IFormElementRenderer<TextBox>>();
            _serviceCollection.AddTransient<IFormElementRenderer<TextBox>>(provider => formElementRenderer);

            var sut = GetSUT();
            var renderer = sut.GetPropertyRenderer(new TextBox());

            renderer.Should().Be(formElementRenderer);
        }
    }
}