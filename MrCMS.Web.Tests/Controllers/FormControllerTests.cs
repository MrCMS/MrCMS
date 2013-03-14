using System.Web;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Services;
using MrCMS.Web.Controllers;
using MrCMS.Web.Tests.Stubs;
using Xunit;

namespace MrCMS.Web.Tests.Controllers
{
    public class FormControllerTests
    {
        private IFormService _formService;
        private FormController _formController;

        public FormControllerTests()
        {
            _formService = A.Fake<IFormService>();
            _formController = new FormController(_formService) {RequestMock = A.Fake<HttpRequestBase>()};
            _formController.ReferrerOverride = "test-redirect";
        }

        [Fact]
        public void FormController_Save_CallsFormServiceSaveFormDataWithPassedObjects()
        {
            var stubWebpage = new StubWebpage();

            var result = _formController.Save(stubWebpage);

            A.CallTo(() => _formService.SaveFormData(stubWebpage, _formController.Request)).MustHaveHappened();
        }

        [Fact]
        public void FormController_Save_SetsTempDataFormSubmittedToTrue()
        {
            var stubWebpage = new StubWebpage();

            var result = _formController.Save(stubWebpage);

            _formController.TempData["form-submitted"].Should().Be(true);
        }

        [Fact]
        public void FormController_Save_ReturnsRedirectToTheReferrer()
        {
            var stubWebpage = new StubWebpage();

            var result = _formController.Save(stubWebpage);

            result.Should().BeOfType<RedirectResult>();
            result.As<RedirectResult>().Url.Should().Be("test-redirect");
        }
    }
}