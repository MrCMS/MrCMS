using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Admin.Controllers;
using MrCMS.Web.Admin.Models;
using MrCMS.Web.Admin.Services;
using MrCMS.Web.Admin.Tests.Stubs;
using MrCMS.Web.Apps.Core.Pages;
using X.PagedList;
using Xunit;

namespace MrCMS.Web.Admin.Tests.Controllers
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
            var form = new Form();

            var result = _formController.ClearFormData(form);

            result.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void FormController_ClearFormDataPOST_ShouldCallClearFormData()
        {
            var form = new Form();

            _formController.ClearFormData_POST(form);

            A.CallTo(() => _formAdminService.ClearFormData(form)).MustHaveHappened();
        }

        [Fact]
        public void FormController_ClearFormDataPOST_ShouldRedirectToEditWebpage()
        {
            var form = new Form();

            var result = _formController.ClearFormData_POST(form);

            result.ActionName.Should().Be("Edit");
        }

        [Fact]
        public void FormController_ClearFormDataPOST_ShouldReturnRedirectToActionResult()
        {
            var form = new Form();

            var result = _formController.ClearFormData_POST(form);

            result.Should().BeOfType<RedirectToActionResult>();
        }

        [Fact]
        public void FormController_ExportFormData_ShouldCallExportFormData()
        {
            var form = new Form();

            _formController.ExportFormData(form);

            A.CallTo(() => _formAdminService.ExportFormData(form)).MustHaveHappened();
        }

        [Fact]
        public void FormController_ExportFormData_ShouldReturnFileResult()
        {
            var form = new Form();

            var result = _formController.ExportFormData(form);

            result.Should().BeOfType<FileContentResult>();
        }

        [Fact]
        public void FormController_Posting_ReturnsTheResultOfTheCallToGetFormPostings()
        {
            var form = new Form();
            var postingsModel = new PostingsModel(new StaticPagedList<FormPosting>(new FormPosting[0], 1, 1, 0), 1);
            A.CallTo(() => _formAdminService.GetFormPostings(form, 1, null)).Returns(postingsModel);
            _formController.Postings(form, 1, null).As<PartialViewResult>().Model.Should().Be(postingsModel);
        }

        [Fact]
        public void FormController_Postings_CallsFormServiceGetFormPostingsWithPassedArguments()
        {
            var form = new Form();
            _formController.Postings(form, 1, null);

            A.CallTo(() => _formAdminService.GetFormPostings(form, 1, null)).MustHaveHappened();
        }

        [Fact]
        public void FormController_Postings_ReturnsAPartialViewResult()
        {
            _formController.Postings(new Form(), 1, null).Should().BeOfType<PartialViewResult>();
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