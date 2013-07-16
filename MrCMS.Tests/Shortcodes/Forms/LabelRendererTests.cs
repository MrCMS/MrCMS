using FluentAssertions;
using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Shortcodes.Forms;
using Xunit;

namespace MrCMS.Tests.Shortcodes.Forms
{
    public class LabelRendererTests
    {
        private readonly LabelRenderer _labelRenderer;

        public LabelRendererTests()
        {
            _labelRenderer = new LabelRenderer();
        }

        [Fact]
        public void TextAreaRenderer_AppendLabel_ShouldHaveTagNameLabel()
        {
            var appendLabel = _labelRenderer.AppendLabel(new TextArea());

            appendLabel.TagName.Should().Be("label");
        }

        [Fact]
        public void TextAreaRenderer_AppendLabel_ShouldHaveForAsHtmlId()
        {
            var appendLabel = _labelRenderer.AppendLabel(new TextArea { HtmlId = "test-id" });

            appendLabel.Attributes["for"].Should().Be("test-id");
        }

        [Fact]
        public void TextAreaRenderer_AppendLabel_ShouldHaveTextAsNameIfNoLabelTextIsSet()
        {
            var appendLabel = _labelRenderer.AppendLabel(new TextArea { Name = "Test Name" });

            appendLabel.InnerHtml.Should().Be("Test Name");
        }

        [Fact]
        public void TextAreaRenderer_AppendLabel_ShouldUseLabelTextOverNameIfItIsNotNull()
        {
            var appendLabel = _labelRenderer.AppendLabel(
                new TextArea { Name = "Test Name", LabelText = "Test Label Text" });

            appendLabel.InnerHtml.Should().Be("Test Label Text");
        }

    }
}