using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
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
            var sut = GetSUT(new DummyShortcodeRenderer("test", new HtmlString("result")));

            sut.CanRender("test").Should().BeTrue();
        }

        [Fact]
        public void Render_ReturnsEmptyStringIfNoMatchingRenderers()
        {
            var sut = GetSUT();

            sut.Render(null, "test", null).Should().Be(HtmlString.Empty);
        }

        [Theory, AutoFakeItEasyData]
        public void Render_ReturnsResultForMatchingRendererWithPassedVariables(
            IHtmlHelper helper,
            Dictionary<string, string> attributes
        )
        {
            var fakeRenderer = A.Fake<IShortcodeRenderer>();
            A.CallTo(() => fakeRenderer.TagName).Returns("test");
            var content = new HtmlString("rendered content");
            A.CallTo(() => fakeRenderer.Render(helper, attributes)).Returns(content);
            var sut = GetSUT(fakeRenderer);

            sut.Render(helper, "test", attributes).Should().Be(content);
        }

        private static RenderShortcode GetSUT(params IShortcodeRenderer[] renderers)
        {
            return new RenderShortcode(renderers);
        }
        public class DummyShortcodeRenderer : IShortcodeRenderer
        {
            private readonly IHtmlContent _renderResult;

            public DummyShortcodeRenderer(string tagName, IHtmlContent renderResult)
            {
                _renderResult = renderResult;
                TagName = tagName;
            }

            public string TagName { get; }

            public IHtmlContent Render(IHtmlHelper helper, Dictionary<string, string> attributes) => _renderResult;
        }
    }
}