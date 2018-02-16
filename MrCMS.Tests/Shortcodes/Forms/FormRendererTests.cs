using FakeItEasy;
using FluentAssertions;
using MrCMS.Shortcodes.Forms;
using MrCMS.Tests.Stubs;
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
            A.CallTo(() => _customFormRenderer.GetForm(_htmlHelper, stubWebpage, formSubmittedStatus))
                .Returns("custom-form");

            var renderForm = _formRenderingManager.RenderForm(_htmlHelper, stubWebpage, formSubmittedStatus);

            renderForm.Should().Be("custom-form");
        }

        [Fact]
        public void FormRenderer_RenderForm_IfWebpageIsNullReturnsEmptyString()
        {
            var renderForm =
                _formRenderingManager.RenderForm(_htmlHelper, null, new FormSubmittedStatus(false, null, null));

            renderForm.Should().Be("");
        }

        [Fact]
        public void FormRenderer_RenderForm_WhenFormDesignIsEmptyReturnsResultOfIDefaultFormRenderer()
        {
            var stubWebpage = new StubWebpage();
            var formSubmittedStatus = new FormSubmittedStatus(false, null, null);
            A.CallTo(() => _defaultFormRenderer.GetDefault(_htmlHelper, stubWebpage, formSubmittedStatus))
                .Returns("test-default");

            var renderForm = _formRenderingManager.RenderForm(_htmlHelper, stubWebpage, formSubmittedStatus);

            renderForm.Should().Be("test-default");
        }
    }
}