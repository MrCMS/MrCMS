using FluentAssertions;
using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Shortcodes.Forms;
using Xunit;

namespace MrCMS.Tests.Shortcodes.Forms
{
    public class DropDownListOptionRendererTests
    {
        private DropDownListOptionRenderer _dropDownListOptionRenderer;

        public DropDownListOptionRendererTests()
        {
            _dropDownListOptionRenderer = new DropDownListOptionRenderer();
        }

        [Fact]
        public void DropDownListOptionRenderer_GetOption_ReturnsATagNameOfOption()
        {
            var tagBuilder = _dropDownListOptionRenderer.GetOption(new FormListOption(), "test");

            tagBuilder.TagName.Should().Be("option");
        }

        [Fact]
        public void DropDownListOptionRendere_GetOption_ValueAttributeShouldComeFromPassedOption()
        {
            var tagBuilder = _dropDownListOptionRenderer.GetOption(new FormListOption{Value = "test-value"}, "test");

            tagBuilder.Attributes["value"].Should().Be("test-value");
        }

        [Fact]
        public void DropDownListOptionRendere_GetOption_IfValuePassedMatchesSelectedItShouldHaveSelectedAttribute()
        {
            var tagBuilder = _dropDownListOptionRenderer.GetOption(new FormListOption{Value = "test-value"}, "test-value");

            tagBuilder.Attributes["selected"].Should().Be("selected");
        }

        [Fact]
        public void DropDownListOptionRendere_GetOption_IfThePassedValueIsNullAndTheOptionIsSelectedItShouldHaveASelectedAttribute()
        {
            var tagBuilder = _dropDownListOptionRenderer.GetOption(new FormListOption{Selected = true}, null);

            tagBuilder.Attributes["selected"].Should().Be("selected");
        }

        [Fact]
        public void DropDownListOptionRendere_GetOption_IfThePassedValueIsNotNullAndTheOptionIsSelectedItShouldNotHaveASelectedAttribute()
        {
            var tagBuilder = _dropDownListOptionRenderer.GetOption(new FormListOption{Selected = true}, "test");

            tagBuilder.Attributes.ContainsKey("selected").Should().BeFalse();
        }
    }
}