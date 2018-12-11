using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Admin.Controllers;
using MrCMS.Web.Apps.Admin.Models;
using MrCMS.Web.Apps.Admin.Services;
using MrCMS.Web.Apps.Admin.Tests.Stubs;
using MrCMS.Web.Apps.Core.Pages;
using X.PagedList;
using Xunit;

namespace MrCMS.Web.Apps.Admin.Tests.Controllers
{
    public class FormControllerTests
    {
        public FormControllerTests()
        {
            _formAdminService = A.Fake<IFormAdminService>();
            _formController = new FormController(_formAdminService);
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

            result.ActionName.Should().Be("Edit");
        }

        [Fact]
        public void FormController_ClearFormDataPOST_ShouldReturnRedirectToActionResult()
        {
            var stubWebpage = new StubWebpage();

            var result = _formController.ClearFormData_POST(stubWebpage);

            result.Should().BeOfType<RedirectToActionResult>();
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
            var postingsModel = new PostingsModel(new StaticPagedList<FormPosting>(new FormPosting[0], 1, 1, 0), 1);
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