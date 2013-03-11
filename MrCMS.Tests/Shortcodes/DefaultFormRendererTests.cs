using System.Collections.Generic;
using FakeItEasy;
using FakeItEasy.Core;
using FluentAssertions;
using MrCMS.DbConfiguration.Configuration;
using MrCMS.Entities.Documents.Web;
using MrCMS.IoC;
using MrCMS.Shortcodes;
using MrCMS.Tests.Stubs;
using MrCMS.Website;
using Ninject;
using Ninject.MockingKernel;
using Xunit;
using System.Linq;

namespace MrCMS.Tests.Shortcodes
{
    public class DefaultFormRendererTests
    {
        private DefaultFormRenderer _defaultFormRenderer;
        private IElementRendererManager _elementRendererManager;

        public DefaultFormRendererTests()
        {
            _elementRendererManager = A.Fake<IElementRendererManager>();
            _defaultFormRenderer = new DefaultFormRenderer(_elementRendererManager);
        }

        [Fact]
        public void DefaultFormRenderer_GetDefault_ShouldReturnAnEmptyStringIfThereAreNoProperties()
        {
            var stubWebpage = new StubWebpage();

            var @default = _defaultFormRenderer.GetDefault(stubWebpage);

            @default.Should().Be(string.Empty);
        }

        [Fact]
        public void DefaultFormRenderer_GetDefault_ShouldReturnAnEmptyStringIfWebpageIsNull()
        {
            var @default = _defaultFormRenderer.GetDefault(null);

            @default.Should().Be(string.Empty);
        }

        [Fact]
        public void DefaultFormRenderer_GetDefault_ShouldCallGetElementRendererOnEachProperty()
        {
            var textBox = new TextBox { Name = "test-1" };
            var stubWebpage = new StubWebpage
                                  {
                                      FormProperties = new List<FormProperty> { textBox }
                                  };

            _defaultFormRenderer.GetDefault(stubWebpage);

            A.CallTo(() => _elementRendererManager.GetElementRenderer<FormProperty>(textBox)).MustHaveHappened();
        }


        [Fact]
        public void DefaultFormRenderer_GetDefault_ShouldCallAppendLabelOnElementRendererForEachProperty()
        {
            var textBox = new TextBox { Name = "test-1" };
            var stubWebpage = new StubWebpage
                                  {
                                      FormProperties = new List<FormProperty> { textBox }
                                  };
            var formElementRenderer = A.Fake<IFormElementRenderer<FormProperty>>();
            A.CallTo(() => _elementRendererManager.GetElementRenderer<FormProperty>(textBox))
             .Returns(formElementRenderer);

            _defaultFormRenderer.GetDefault(stubWebpage);

            A.CallTo(() => formElementRenderer.AppendLabel(textBox)).MustHaveHappened();
        }

        [Fact]
        public void DefaultFormRenderer_GetDefault_ShouldCallAppendControlOnElementRenderer()
        {
            var textBox = new TextBox { Name = "test-1" };
            var stubWebpage = new StubWebpage
            {
                FormProperties = new List<FormProperty> { textBox }
            };
            var formElementRenderer = A.Fake<IFormElementRenderer<FormProperty>>();
            A.CallTo(() => _elementRendererManager.GetElementRenderer<FormProperty>(textBox))
             .Returns(formElementRenderer);

            _defaultFormRenderer.GetDefault(stubWebpage);

            A.CallTo(() => formElementRenderer.AppendElement(textBox)).MustHaveHappened();
        }

        [Fact]
        public void DefaultFormRenderer_GetDefault_ShouldCallRenderLabelThenRenderElementForEachProperty()
        {
            var textBox1 = new TextBox { Name = "test-1" };
            var textBox2 = new TextBox { Name = "test-2" };

            var stubWebpage = new StubWebpage
            {
                FormProperties = new List<FormProperty> { textBox1, textBox2 }
            };
            var formElementRenderer = A.Fake<IFormElementRenderer<FormProperty>>();
            A.CallTo(() => _elementRendererManager.GetElementRenderer<FormProperty>(textBox1))
             .Returns(formElementRenderer);
            A.CallTo(() => _elementRendererManager.GetElementRenderer<FormProperty>(textBox2))
             .Returns(formElementRenderer);

            _defaultFormRenderer.GetDefault(stubWebpage);

            List<ICompletedFakeObjectCall> completedFakeObjectCalls = Fake.GetCalls(formElementRenderer).ToList();

            completedFakeObjectCalls[0].Method.Name.Should().Be("AppendLabel");
            completedFakeObjectCalls[0].Arguments[0].Should().Be(textBox1);
            completedFakeObjectCalls[1].Method.Name.Should().Be("AppendElement");
            completedFakeObjectCalls[1].Arguments[0].Should().Be(textBox1);
            completedFakeObjectCalls[2].Method.Name.Should().Be("AppendLabel");
            completedFakeObjectCalls[2].Arguments[0].Should().Be(textBox2);
            completedFakeObjectCalls[3].Method.Name.Should().Be("AppendElement");
            completedFakeObjectCalls[3].Arguments[0].Should().Be(textBox2);
        }
    }
}