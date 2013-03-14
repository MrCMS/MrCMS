using FakeItEasy;
using FluentAssertions;
using MrCMS.Shortcodes.Forms;
using MrCMS.Tests.Stubs;
using Xunit;

namespace MrCMS.Tests.Shortcodes.Forms
{
    public class FormRenderingManagerTests
    {
        private FormRenderingManager _formRenderingManager;
        private IDefaultFormRenderer _defaultFormRenderer;
        private ICustomFormRenderer _customFormRenderer;

        public FormRenderingManagerTests()
        {
            _defaultFormRenderer = A.Fake<IDefaultFormRenderer>();
            _customFormRenderer = A.Fake<ICustomFormRenderer>();
            _formRenderingManager = new FormRenderingManager(_defaultFormRenderer, _customFormRenderer);
        }

        [Fact]
        public void FormRenderer_RenderForm_WhenFormDesignIsEmptyReturnsResultOfIDefaultFormRenderer()
        {
            var stubWebpage = new StubWebpage();
            var formSubmittedStatus = new FormSubmittedStatus(false, null, null);
            A.CallTo(() => _defaultFormRenderer.GetDefault(stubWebpage, formSubmittedStatus)).Returns("test-default");

            var renderForm = _formRenderingManager.RenderForm(stubWebpage, formSubmittedStatus);

            renderForm.Should().Be("test-default");
        }

        [Fact]
        public void FormRenderer_RenderForm_IfWebpageIsNullReturnsEmptyString()
        {
            var renderForm = _formRenderingManager.RenderForm(null, new FormSubmittedStatus(false, null, null));

            renderForm.Should().Be("");
        }

        [Fact]
        public void FormRenderer_RenderForm_IfFormDesignHasValueReturnResultCustomRendererGetForm()
        {
            var stubWebpage = new StubWebpage {FormDesign = "form-design-data"};
            var formSubmittedStatus = new FormSubmittedStatus(false, null, null);
            A.CallTo(() => _customFormRenderer.GetForm(stubWebpage, formSubmittedStatus)).Returns("custom-form");

            var renderForm = _formRenderingManager.RenderForm(stubWebpage, formSubmittedStatus);

            renderForm.Should().Be("custom-form");
        }
    }
}