using System.Web;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin;
using MrCMS.Web.Areas.Admin.Controllers;
using MrCMS.Web.Tests.Stubs;
using Xunit;

namespace MrCMS.Web.Tests.Areas.Admin.Controllers
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
        public void FormController_ClearFormData_ShouldReturnPartialViewResult()
        {
            var stubWebpage = new StubWebpage();

            var result = _formController.ClearFormData(stubWebpage);

            result.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void FormController_ClearFormDataPOST_ShouldReturnRedirectToRouteResult()
        {
            var stubWebpage = new StubWebpage();

            var result = _formController.ClearFormData_POST(stubWebpage);

            result.Should().BeOfType<RedirectToRouteResult>();
        }

        [Fact]
        public void FormController_ClearFormDataPOST_ShouldRedirectToEditWebpage()
        {
            var stubWebpage = new StubWebpage();

            var result = _formController.ClearFormData_POST(stubWebpage);

            result.RouteValues["action"].Should().Be("Edit");
        }

        [Fact]
        public void FormController_ClearFormDataPOST_ShouldCallClearFormData()
        {
            var stubWebpage = new StubWebpage();

            _formController.ClearFormData_POST(stubWebpage);

            A.CallTo(() => _formService.ClearFormData(stubWebpage)).MustHaveHappened();
        }

        [Fact]
        public void FormController_ExportFormData_ShouldReturnFileResult()
        {
            var stubWebpage = new StubWebpage();

            var result = _formController.ExportFormData(stubWebpage);

            result.Should().BeOfType<FileContentResult>();
        }

        [Fact]
        public void FormController_ExportFormData_ShouldCallExportFormData()
        {
            var stubWebpage = new StubWebpage();

            _formController.ExportFormData(stubWebpage);

            A.CallTo(() => _formService.ExportFormData(stubWebpage)).MustHaveHappened();
        }
    }
}