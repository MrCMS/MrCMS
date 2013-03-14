using FluentAssertions;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Shortcodes.Forms;
using Xunit;

namespace MrCMS.Tests.Shortcodes.Forms
{
    public class TextAreaRendererTests
    {
        private TextAreaRenderer _textBoxRenderer;
        private string _existingValue = "";

        public TextAreaRendererTests()
        {
            _textBoxRenderer = new TextAreaRenderer();
        }

     
        [Fact]
        public void TextAreaRenderer_AppendElement_ShouldHaveTagNameOfTextArea()
        {
            var appendElement = _textBoxRenderer.AppendElement(new TextArea(), _existingValue);

            appendElement.TagName.Should().Be("textarea");
        }

        [Fact]
        public void TextAreaRenderer_AppendElement_ShouldHaveNameOfTextAreaName()
        {
            var appendElement = _textBoxRenderer.AppendElement(
                new TextArea { Name = "test name" }, _existingValue);

            appendElement.Attributes["name"].Should().Be("test name");
        }

        [Fact]
        public void TextAreaRenderer_AppendElement_ShouldSetHtmlIdIfItIsSet()
        {
            var appendElement = _textBoxRenderer.AppendElement(
                new TextArea { HtmlId = "test-id" }, _existingValue);

            appendElement.Attributes["id"].Should().Be("test-id");
        }

        [Fact]
        public void TextAreaRenderer_IsSelfClosing_ShouldBeFalse()
        {
            _textBoxRenderer.IsSelfClosing.Should().BeFalse();
        }

        [Fact]
        public void TextAreaRenderer_AppendElement_ShouldAppendDataValTrueIfIsRequired()
        {
            var appendElement = _textBoxRenderer.AppendElement(
                new TextArea { Required = true }, _existingValue);

            appendElement.Attributes["data-val"].Should().Be("true");
        }

        [Fact]
        public void TextAreaRenderer_AppendElement_ShouldAppendDataValRequiredIfIsRequiredUsingTheLabelText()
        {
            var appendElement = _textBoxRenderer.AppendElement(
                new TextArea { Required = true, LabelText = "Test Label" }, _existingValue);

            appendElement.Attributes["data-val-required"].Should().Be("The field Test Label is required");
        }

        [Fact]
        public void TextAreaRenderer_AppendElement_ShouldAppendDataValRequiredIfIsRequiredUsingTheNameIfLabelTextIsMissing()
        {
            var appendElement = _textBoxRenderer.AppendElement(
                new TextArea { Required = true, Name = "test-name" }, _existingValue);

            appendElement.Attributes["data-val-required"].Should().Be("The field test-name is required");
        }

        [Fact]
        public void TextAreaRenderer_AppendElement_ShouldAppendNeitherDataAttributesIfRequiredIsFalse()
        {
            var appendElement = _textBoxRenderer.AppendElement(
                new TextArea { Required = false }, _existingValue);

            appendElement.Attributes.ContainsKey("data-val").Should().BeFalse();
            appendElement.Attributes.ContainsKey("data-val-required").Should().BeFalse();
        }
    }
}