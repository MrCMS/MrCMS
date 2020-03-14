using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Web;
using MrCMS.Shortcodes.Forms;
using MrCMS.TestSupport;
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
            var form = new Form { FormDesign = "form-design-data" };
            var formSubmittedStatus = new FormSubmittedStatus(false, null, null);
            var customForm = new HtmlString("custom-form");
            A.CallTo(() => _customFormRenderer.GetForm(_htmlHelper, form, formSubmittedStatus))
                .Returns(customForm);

            var renderForm = _formRenderingManager.RenderForm(_htmlHelper, form, formSubmittedStatus);

            renderForm.Should().Be(customForm);
        }

        [Fact]
        public async Task FormRenderer_RenderForm_IfFormIsNullReturnsEmptyString()
        {
            var renderForm =
             await   _formRenderingManager.RenderForm(_htmlHelper, null, new FormSubmittedStatus(false, null, null));

            renderForm.AsAString().Should().Be("");
        }

        [Fact]
        public void FormRenderer_RenderForm_WhenFormDesignIsEmptyReturnsResultOfIDefaultFormRenderer()
        {
            var form = new Form();
            var formSubmittedStatus = new FormSubmittedStatus(false, null, null);
            var value = new HtmlString("test-default");
            A.CallTo(() => _defaultFormRenderer.GetDefault(_htmlHelper, form, formSubmittedStatus))
                .Returns(value);

            var renderForm = _formRenderingManager.RenderForm(_htmlHelper, form, formSubmittedStatus);

            renderForm.Should().Be(value);
        }
    }
}