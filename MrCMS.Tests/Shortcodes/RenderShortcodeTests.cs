using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Shortcodes;
using MrCMS.Website;
using Xunit;
using Xunit.Extensions;

namespace MrCMS.Tests.Shortcodes
{
    public class RenderShortcodeTests
    {
        [Fact]
        public void CanRender_ReturnsFalseIfThereAreNoMatchingRenderers()
        {
            var sut = GetSUT();

            sut.CanRender("test").Should().BeFalse();
        }

        [Fact]
        public void CanRender_ReturnsTrueIfMatchingRenderer()
        {
            var sut = GetSUT(new DummyShortcodeRenderer("test", "result"));

            sut.CanRender("test").Should().BeTrue();
        }

        [Fact]
        public void Render_ReturnsEmptyStringIfNoMatchingRenderers()
        {
            var sut = GetSUT();

            sut.Render(null, "test", null).Should().Be(string.Empty);
        }

        [Theory, AutoFakeItEasyData]
        public void Render_ReturnsResultForMatchingRendererWithPassedVariables(
            IHtmlHelper helper,
            Dictionary<string, string> attributes
        )
        {
            var fakeRenderer = A.Fake<IShortcodeRenderer>();
            A.CallTo(() => fakeRenderer.TagName).Returns("test");
            A.CallTo(() => fakeRenderer.Render(helper, attributes)).Returns("rendered content");
            var sut = GetSUT(fakeRenderer);

            sut.Render(helper, "test", attributes).Should().Be("rendered content");
        }

        private static RenderShortcode GetSUT(params IShortcodeRenderer[] renderers)
        {
            return new RenderShortcode(renderers);
        }
        public class DummyShortcodeRenderer : IShortcodeRenderer
        {
            private readonly string _renderResult;

            public DummyShortcodeRenderer(string tagName, string renderResult)
            {
                _renderResult = renderResult;
                TagName = tagName;
            }

            public string TagName { get; }
            public string Render(IHtmlHelper helper, Dictionary<string, string> attributes) => _renderResult;
        }
    }
}