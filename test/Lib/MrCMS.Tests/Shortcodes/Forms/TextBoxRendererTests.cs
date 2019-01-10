using FluentAssertions;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Settings;
using MrCMS.Shortcodes.Forms;
using Xunit;

namespace MrCMS.Tests.Shortcodes.Forms
{
    public class TextBoxRendererTests
    {
        private readonly TextBoxRenderer _textBoxRenderer;
        private string _existingValue = "";

        public TextBoxRendererTests()
        {
            _textBoxRenderer = new TextBoxRenderer();
        }

        [Fact]
        public void TextBoxRenderer_AppendElement_ShouldHaveTagNameOfInput()
        {
            var appendElement = _textBoxRenderer.AppendElement(new TextBox(), _existingValue, FormRenderingType.Bootstrap2);

            appendElement.TagName.Should().Be("input");
        }

        [Fact]
        public void TextBoxRenderer_AppendElement_ShouldHaveTypeOfText()
        {
            var appendElement = _textBoxRenderer.AppendElement(new TextBox(), _existingValue, FormRenderingType.Bootstrap2);

            appendElement.Attributes["type"].Should().Be("text");
        }

        [Fact]
        public void TextBoxRenderer_AppendElement_ShouldHaveNameOfTextBoxName()
        {
            var appendElement = _textBoxRenderer.AppendElement(
                new TextBox { Name = "test name" }, _existingValue, FormRenderingType.Bootstrap2);

            appendElement.Attributes["name"].Should().Be("test name");
        }

        [Fact]
        public void TextBoxRenderer_AppendElement_ShouldSetHtmlIdIfItIsSet()
        {
            var appendElement = _textBoxRenderer.AppendElement(
                new TextBox { HtmlId = "test-id" }, _existingValue, FormRenderingType.Bootstrap2);

            appendElement.Attributes["id"].Should().Be("test-id");
        }

        [Fact]
        public void TextBoxRenderer_IsSelfClosing_ShouldBeTrue()
        {
            _textBoxRenderer.IsSelfClosing.Should().BeTrue();
        }

        [Fact]
        public void TextBoxRenderer_AppendElement_ShouldAppendDataValTrueIfIsRequired()
        {
            var appendElement = _textBoxRenderer.AppendElement(
                new TextBox { Required = true }, _existingValue, FormRenderingType.Bootstrap2);

            appendElement.Attributes["data-val"].Should().Be("true");
        }

        [Fact]
        public void TextBoxRenderer_AppendElement_ShouldAppendDataValRequiredIfIsRequiredUsingTheLabelText()
        {
            var appendElement = _textBoxRenderer.AppendElement(
                new TextBox { Required = true, LabelText = "Test Label" }, _existingValue, FormRenderingType.Bootstrap2);

            appendElement.Attributes["data-val-required"].Should().Be("The field Test Label is required");
        }

        [Fact]
        public void TextBoxRenderer_AppendElement_ShouldAppendDataValRequiredIfIsRequiredUsingTheNameIfLabelTextIsMissing()
        {
            var appendElement = _textBoxRenderer.AppendElement(
                new TextBox { Required = true, Name = "test-name" }, _existingValue, FormRenderingType.Bootstrap2);

            appendElement.Attributes["data-val-required"].Should().Be("The field test-name is required");
        }

        [Fact]
        public void TextBoxRenderer_AppendElement_ShouldAppendNeitherDataAttributesIfRequiredIsFalse()
        {
            var appendElement = _textBoxRenderer.AppendElement(
                new TextBox { Required = false }, _existingValue, FormRenderingType.Bootstrap2);

            appendElement.Attributes.ContainsKey("data-val").Should().BeFalse();
            appendElement.Attributes.ContainsKey("data-val-required").Should().BeFalse();
        }

        [Fact]
        public void TextBoxRenderer_AppendElement_ShouldSetCssClassIfItIsSet()
        {
            var appendElement = _textBoxRenderer.AppendElement(
                new TextBox { CssClass = "css-class" }, _existingValue, FormRenderingType.Bootstrap2);

            appendElement.Attributes["class"].Should().Be("css-class");

        }
    }
}