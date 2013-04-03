using System.Web;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Documents.Web;
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
        private IDocumentService _documentService;

        public FormControllerTests()
        {
            _documentService = A.Fake<IDocumentService>();
            _formService = A.Fake<IFormService>();
            _formController = new FormController(_documentService, _formService)
                                  {
                                      RequestMock =
                                          A.Fake<HttpRequestBase>(),
                                      ReferrerOverride = "http://www.example.com/test-redirect"
                                  };
        }

        [Fact]
        public void FormController_Save_CallsFormServiceSaveFormDataWithPassedObjects()
        {
            var stubWebpage = new StubWebpage();
            A.CallTo(() => _documentService.GetDocument<Webpage>(1)).Returns(stubWebpage);
            var result = _formController.Save(1);

            A.CallTo(() => _formService.SaveFormData(stubWebpage, _formController.Request)).MustHaveHappened();
        }

        [Fact]
        public void FormController_Save_SetsTempDataFormSubmittedToTrue()
        {
            var stubWebpage = new StubWebpage();

            A.CallTo(() => _documentService.GetDocument<Webpage>(1)).Returns(stubWebpage);
            var result = _formController.Save(1);

            _formController.TempData["form-submitted"].Should().Be(true);
        }

        [Fact]
        public void FormController_Save_ReturnsRedirectToTheReferrer()
        {
            var stubWebpage = new StubWebpage();

            A.CallTo(() => _documentService.GetDocument<Webpage>(1)).Returns(stubWebpage);
            var result = _formController.Save(1);

            result.Should().BeOfType<RedirectResult>();
            result.As<RedirectResult>().Url.Should().Be("http://www.example.com/test-redirect");
        }
    }
}