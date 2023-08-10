﻿using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Settings;
using MrCMS.Shortcodes.Forms;
using MrCMS.TestSupport;
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
            var element = _dropDownListRenderer.AppendElement(new DropDownList(), null, FormRenderingType.Bootstrap5);

            element.TagName.Should().Be("select");
        }

        [Fact]
        public void DropDownListRenderer_AppendElement_NameShouldBeFormPropertyName()
        {
            var element = _dropDownListRenderer.AppendElement(new DropDownList { Name = "test-name" }, null, FormRenderingType.Bootstrap5);

            element.Attributes["name"].Should().Be("test-name");
        }

        [Fact]
        public void DropDownListRenderer_AppendElement_IdShouldBeIdIfItIsSet()
        {
            var element = _dropDownListRenderer.AppendElement(new DropDownList { HtmlId = "test-id" }, null, FormRenderingType.Bootstrap5);

            element.Attributes["id"].Should().Be("test-id");
        }

        [Fact]
        public void DropDownListRenderer_AppendElement_IdShouldBeNameIfIdIsNotSet()
        {
            var element = _dropDownListRenderer.AppendElement(new DropDownList { Name = "test-name" }, null, FormRenderingType.Bootstrap5);

            element.Attributes["id"].Should().Be("test-name");
        }

        [Fact]
        public void DropDownListRenderer_AppendElement_IfItIsRequiredShouldHaveDataValTrue()
        {
            var element = _dropDownListRenderer.AppendElement(new DropDownList { Required = true }, null, FormRenderingType.Bootstrap5);

            element.Attributes["data-val"].Should().Be("true");
        }

        [Fact]
        public void DropDownListRenderer_AppendElement_ShouldHaveMessageSet()
        {
            var element = _dropDownListRenderer.AppendElement(new DropDownList { Required = true, Name = "test-name" }, null, FormRenderingType.Bootstrap5);

            element.Attributes["data-val-required"].Should().Be("The field test-name is required");
        }

        [Fact]
        public void DropDownListRenderer_AppendElement_ShouldHaveMessageSetWithLabelTextIfItIsSet()
        {
            var element = _dropDownListRenderer.AppendElement(new DropDownList { Required = true, Name = "test-name", LabelText = "Test Name" }, null, FormRenderingType.Bootstrap5);

            element.Attributes["data-val-required"].Should().Be("The field Test Name is required");
        }

        [Fact]
        public void DropDownListRenderer_AppendElement_CssClassShouldBeSetIfItExists()
        {
            var element = _dropDownListRenderer.AppendElement(new DropDownList { CssClass = "css-class" }, null, FormRenderingType.Bootstrap5);

            element.Attributes["class"].Should().Be("css-class form-select");
        }

        [Fact]
        public void DropDownListRenderer_AppendElement_ForEachOptionInFormPropertyOptionRendererShouldBeCalled()
        {
            var dropDownList = new DropDownList
            {
                Options =
                    new List<FormListOption> { new FormListOption(), new FormListOption(), new FormListOption() }
            };

            _dropDownListRenderer.AppendElement(dropDownList, null, FormRenderingType.Bootstrap5);

            A.CallTo(() => _dropDownListOptionRenderer.GetOption(A<FormListOption>.Ignored, null))
                .MustHaveHappenedANumberOfTimesMatching(i => i == 3);
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
            var appendElement = _dropDownListRenderer.AppendElement(dropDownList, null, FormRenderingType.Bootstrap5);

            appendElement.InnerHtml.AsAString().Should().Be("<option></option>");
        }

        [Fact]
        public void DropDownListRenderer_IsSelfClosing_ShouldBeFalse()
        {
            _dropDownListRenderer.IsSelfClosing.Should().BeFalse();
        }
    }
}