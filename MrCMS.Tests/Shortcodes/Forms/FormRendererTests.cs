using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Shortcodes.Forms;
using MrCMS.Tests.Stubs;
using MrCMS.Tests.TestSupport;
using MrCMS.Website;
using Xunit;

namespace MrCMS.Tests.Shortcodes.Forms
{
    public class FormRenderingManagerTests
    {
        public FormRenderingManagerTests()
        {
            _defaultFormRenderer = A.Fake<IDefaultFormRenderer>();
            _customFormRenderer = A.Fake<ICustomFormRenderer>();
            _htmlHelper = A.Fake<IHtmlHelper>();
            _formRenderingManager = new FormRenderingManager(_defaultFormRenderer, _customFormRenderer);
        }

        private readonly FormRenderingManager _formRenderingManager;
        private readonly IDefaultFormRenderer _defaultFormRenderer;
        private readonly ICustomFormRenderer _customFormRenderer;
        private readonly IHtmlHelper _htmlHelper;

        [Fact]
        public void FormRenderer_RenderForm_IfFormDesignHasValueReturnResultCustomRendererGetForm()
        {
            var stubWebpage = new StubWebpage {FormDesign = "form-design-data"};
            var formSubmittedStatus = new FormSubmittedStatus(false, null, null);
            var customForm = new HtmlString("custom-form");
            A.CallTo(() => _customFormRenderer.GetForm(_htmlHelper, stubWebpage, formSubmittedStatus))
                .Returns(customForm);

            var renderForm = _formRenderingManager.RenderForm(_htmlHelper, stubWebpage, formSubmittedStatus);

            renderForm.Should().Be(customForm);
        }

        [Fact]
        public void FormRenderer_RenderForm_IfWebpageIsNullReturnsEmptyString()
        {
            var renderForm =
                _formRenderingManager.RenderForm(_htmlHelper, null, new FormSubmittedStatus(false, null, null));

            renderForm.AsAString().Should().Be("");
        }

        [Fact]
        public void FormRenderer_RenderForm_WhenFormDesignIsEmptyReturnsResultOfIDefaultFormRenderer()
        {
            var stubWebpage = new StubWebpage();
            var formSubmittedStatus = new FormSubmittedStatus(false, null, null);
            var value = new HtmlString("test-default");
            A.CallTo(() => _defaultFormRenderer.GetDefault(_htmlHelper, stubWebpage, formSubmittedStatus))
                .Returns(value);

            var renderForm = _formRenderingManager.RenderForm(_htmlHelper, stubWebpage, formSubmittedStatus);

            renderForm.Should().Be(value);
        }
    }
}