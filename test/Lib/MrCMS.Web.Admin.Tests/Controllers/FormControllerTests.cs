using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Admin.Controllers;
using MrCMS.Web.Admin.Models;
using MrCMS.Web.Admin.Services;
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

            var result = _formController.ClearFormData(0);

            result.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public async Task FormController_ClearFormDataPOST_ShouldCallClearFormData()
        {
            var form = new Form();
            await _formController.ClearFormData_POST(0);

            A.CallTo(() => _formAdminService.ClearFormData(form)).MustHaveHappened();
        }

        [Fact]
        public async Task FormController_ClearFormDataPOST_ShouldRedirectToEditWebpage()
        {
            var result = await _formController.ClearFormData_POST(0);

            result.ActionName.Should().Be("Edit");
        }

        [Fact]
        public async Task FormController_ClearFormDataPOST_ShouldReturnRedirectToActionResult()
        {
            var result = await _formController.ClearFormData_POST(0);

            result.Should().BeOfType<RedirectToActionResult>();
        }

        [Fact]
        public async Task FormController_ExportFormData_ShouldCallExportFormData()
        {
            var form = new Form();

            await _formController.ExportFormData(0);

            A.CallTo(() => _formAdminService.ExportFormData(form)).MustHaveHappened();
        }

        [Fact]
        public async Task FormController_ExportFormData_ShouldReturnFileResult()
        {

            var result = await _formController.ExportFormData(0);

            result.Should().BeOfType<FileContentResult>();
        }

        [Fact]
        public async Task FormController_Posting_ReturnsTheResultOfTheCallToGetFormPostings()
        {
            var form = new Form();
            var postingsModel = new PostingsModel(new StaticPagedList<FormPosting>(new FormPosting[0], 1, 1, 0), 1);
            A.CallTo(() => _formAdminService.GetFormPostings(form, 1, null)).Returns(postingsModel);
            var postings = await _formController.Postings(0, 1, null);
            postings.As<PartialViewResult>().Model.Should().Be(postingsModel);
        }

        [Fact]
        public async Task FormController_Postings_CallsFormServiceGetFormPostingsWithPassedArguments()
        {
            var form = new Form();
            await _formController.Postings(0, 1, null);

            A.CallTo(() => _formAdminService.GetFormPostings(form, 1, null)).MustHaveHappened();
        }

        [Fact]
        public async Task FormController_Postings_ReturnsAPartialViewResult()
        {
            var postings = await _formController.Postings(0, 1, null);
            postings.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public async Task FormController_ViewPosting_ReturnsTheResultOfTheCallToGetFormPostingAsTheModel()
        {
            var formPosting = new FormPosting();
            var viewPosting = await _formController.ViewPosting(0);
            viewPosting.As<PartialViewResult>().Model.Should().Be(formPosting);
        }

        [Fact]
        public async Task FormController_ViewPosting_ShouldReturnAPartialViewResult()
        {
            var viewPosting = await _formController.ViewPosting(0);
            viewPosting.Should().BeOfType<PartialViewResult>();
        }
    }
}