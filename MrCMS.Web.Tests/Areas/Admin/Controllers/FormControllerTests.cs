using System.Web;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Documents.Web;
using MrCMS.Paging;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Web.Areas.Admin.Controllers;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Web.Tests.Stubs;
using Xunit;

namespace MrCMS.Web.Tests.Areas.Admin.Controllers
{
    public class FormControllerTests
    {
        public FormControllerTests()
        {
            _formAdminService = A.Fake<IFormAdminService>();
            _formController = new FormController(_formAdminService)
            {
                RequestMock =
                    A.Fake<HttpRequestBase>(),
                ReferrerOverride = "http://www.example.com/test-redirect"
            };
        }

        private readonly IFormAdminService _formAdminService;
        private readonly FormController _formController;

        [Fact]
        public void FormController_ClearFormData_ShouldReturnPartialViewResult()
        {
            var stubWebpage = new StubWebpage();

            var result = _formController.ClearFormData(stubWebpage);

            result.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void FormController_ClearFormDataPOST_ShouldCallClearFormData()
        {
            var stubWebpage = new StubWebpage();

            _formController.ClearFormData_POST(stubWebpage);

            A.CallTo(() => _formAdminService.ClearFormData(stubWebpage)).MustHaveHappened();
        }

        [Fact]
        public void FormController_ClearFormDataPOST_ShouldRedirectToEditWebpage()
        {
            var stubWebpage = new StubWebpage();

            var result = _formController.ClearFormData_POST(stubWebpage);

            result.RouteValues["action"].Should().Be("Edit");
        }

        [Fact]
        public void FormController_ClearFormDataPOST_ShouldReturnRedirectToRouteResult()
        {
            var stubWebpage = new StubWebpage();

            var result = _formController.ClearFormData_POST(stubWebpage);

            result.Should().BeOfType<RedirectToRouteResult>();
        }

        [Fact]
        public void FormController_ExportFormData_ShouldCallExportFormData()
        {
            var stubWebpage = new StubWebpage();

            _formController.ExportFormData(stubWebpage);

            A.CallTo(() => _formAdminService.ExportFormData(stubWebpage)).MustHaveHappened();
        }

        [Fact]
        public void FormController_ExportFormData_ShouldReturnFileResult()
        {
            var stubWebpage = new StubWebpage();

            var result = _formController.ExportFormData(stubWebpage);

            result.Should().BeOfType<FileContentResult>();
        }

        [Fact]
        public void FormController_Posting_ReturnsTheResultOfTheCallToGetFormPostings()
        {
            var textPage = new TextPage();
            var postingsModel = new PostingsModel(PagedList<FormPosting>.Empty, 1);
            A.CallTo(() => _formAdminService.GetFormPostings(textPage, 1, null)).Returns(postingsModel);
            _formController.Postings(textPage, 1, null).As<PartialViewResult>().Model.Should().Be(postingsModel);
        }

        [Fact]
        public void FormController_Postings_CallsFormServiceGetFormPostingsWithPassedArguments()
        {
            var textPage = new TextPage();
            _formController.Postings(textPage, 1, null);

            A.CallTo(() => _formAdminService.GetFormPostings(textPage, 1, null)).MustHaveHappened();
        }

        [Fact]
        public void FormController_Postings_ReturnsAPartialViewResult()
        {
            _formController.Postings(new TextPage(), 1, null).Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void FormController_ViewPosting_ReturnsTheResultOfTheCallToGetFormPostingAsTheModel()
        {
            var formPosting = new FormPosting();
            _formController.ViewPosting(formPosting).As<PartialViewResult>().Model.Should().Be(formPosting);
        }

        [Fact]
        public void FormController_ViewPosting_ShouldReturnAPartialViewResult()
        {
            _formController.ViewPosting(new FormPosting()).Should().BeOfType<PartialViewResult>();
        }
    }
}