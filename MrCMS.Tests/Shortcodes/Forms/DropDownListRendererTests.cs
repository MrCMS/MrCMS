using System.Collections.Generic;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Shortcodes.Forms;
using Xunit;

namespace MrCMS.Tests.Shortcodes.Forms
{
    public class DropDownListRendererTests
    {
        private readonly DropDownListRenderer _dropDownListRenderer;
        private readonly IDropDownListOptionRenderer _dropDownListOptionRenderer;

        public DropDownListRendererTests()
        {
            _dropDownListOptionRenderer = A.Fake<IDropDownListOptionRenderer>();
            _dropDownListRenderer = new DropDownListRenderer(_dropDownListOptionRenderer);
        }

        [Fact]
        public void DropDownListRenderer_AppendElement_TagShouldBeOfTypeSelect()
        {
            var element = _dropDownListRenderer.AppendElement(new DropDownList(), null);

            element.TagName.Should().Be("select");
        }

        [Fact]
        public void DropDownListRenderer_AppendElement_NameShouldBeFormPropetyName()
        {
            var element = _dropDownListRenderer.AppendElement(new DropDownList { Name = "test-name" }, null);

            element.Attributes["name"].Should().Be("test-name");
        }

        [Fact]
        public void DropDownListRenderer_AppendElement_IdShouldBeIdIfItIsSet()
        {
            var element = _dropDownListRenderer.AppendElement(new DropDownList { HtmlId = "test-id" }, null);

            element.Attributes["id"].Should().Be("test-id");
        }

        [Fact]
        public void DropDownListRenderer_AppendElement_IdShouldBeNameIfIdIsNotSet()
        {
            var element = _dropDownListRenderer.AppendElement(new DropDownList { Name = "test-name" }, null);

            element.Attributes["id"].Should().Be("test-name");
        }

        [Fact]
        public void DropDownListRenderer_AppendElement_IfItIsRequiredShouldHaveDataValTrue()
        {
            var element = _dropDownListRenderer.AppendElement(new DropDownList { Required = true }, null);

            element.Attributes["data-val"].Should().Be("true");
        }

        [Fact]
        public void DropDownListRenderer_AppendElement_ShouldHaveMessageSet()
        {
            var element = _dropDownListRenderer.AppendElement(new DropDownList { Required = true, Name = "test-name" }, null);

            element.Attributes["data-val-required"].Should().Be("The field test-name is required");
        }

        [Fact]
        public void DropDownListRenderer_AppendElement_ShouldHaveMessageSetWithLabelTextIfItIsSet()
        {
            var element = _dropDownListRenderer.AppendElement(new DropDownList { Required = true, Name = "test-name", LabelText = "Test Name" }, null);

            element.Attributes["data-val-required"].Should().Be("The field Test Name is required");
        }

        [Fact]
        public void DropDownListRenderer_AppendElement_CssClassShouldBeSetIfItExists()
        {
            var element = _dropDownListRenderer.AppendElement(new DropDownList { CssClass = "css-class"}, null);

            element.Attributes["class"].Should().Be("css-class");
        }

        [Fact]
        public void DropDownListRenderer_AppendElement_ForEachOptionInFormPropertyOptionRendererShouldBeCalled()
        {
            var dropDownList = new DropDownList
            {
                Options =
                    new List<FormListOption> { new FormListOption(), new FormListOption(), new FormListOption() }
            };

            _dropDownListRenderer.AppendElement(dropDownList, null);

            A.CallTo(() => _dropDownListOptionRenderer.GetOption(A<FormListOption>.Ignored, null))
             .MustHaveHappened(Repeated.Exactly.Times(3));
        }

        [Fact]
        public void DropDownListRenderer_AppendElement_ForEachOptionInFormPropertyAppendsToInnerHtml()
        {
            var dropDownList = new DropDownList
            {
                Options =
                    new List<FormListOption> { new FormListOption()}
            };

            A.CallTo(() => _dropDownListOptionRenderer.GetOption(A<FormListOption>.Ignored, null))
             .Returns(new TagBuilder("option"));
            var appendElement = _dropDownListRenderer.AppendElement(dropDownList, null);

            appendElement.InnerHtml.Should().Be("<option></option>");
        }

        [Fact]
        public void DropDownListRendere_IsSelfClosing_ShouldBeFalse()
        {
            _dropDownListRenderer.IsSelfClosing.Should().BeFalse();
        }
    }
}