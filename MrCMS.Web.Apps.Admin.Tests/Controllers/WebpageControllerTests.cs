using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.TestSupport;
using MrCMS.Web.Apps.Admin.Controllers;
using MrCMS.Web.Apps.Admin.Models;
using MrCMS.Web.Apps.Admin.Services;
using MrCMS.Web.Apps.Core.Pages;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Options;
using MrCMS.Web.Apps.Admin.Tests.Stubs;
using Xunit;

namespace MrCMS.Web.Apps.Admin.Tests.Controllers
{
    public class WebpageControllerTests : MrCMSTest
    {
        private readonly IUrlValidationService _urlValidationService;
        private readonly IWebpageBaseViewDataService _baseViewDataService;
        private readonly WebpageController _webpageController;
        private readonly IWebpageAdminService _webpageAdminService;
        private IModelBindingHelperAdaptor _modelBindingHelperAdaptor;
        private ISetWebpageAdminViewData _setAdminViewData;

        public WebpageControllerTests()
        {
            _urlValidationService = A.Fake<IUrlValidationService>();
            _baseViewDataService = A.Fake<IWebpageBaseViewDataService>();
            _setAdminViewData = A.Fake<ISetWebpageAdminViewData>();
            _webpageAdminService = A.Fake<IWebpageAdminService>();
            _modelBindingHelperAdaptor = A.Fake<IModelBindingHelperAdaptor>();
            _webpageController = new WebpageController(_webpageAdminService, _baseViewDataService,
                _setAdminViewData,
                _urlValidationService, _modelBindingHelperAdaptor)
            {
                ViewData = ViewDataDictionaryHelper.GetNewDictionary(),
                TempData = new MockTempDataDictionary(),
            };
        }

        [Fact]
        public void WebpageController_AddGet_ShouldReturnAddWebpageModel()
        {
            var addPageModel = A.Dummy<AddWebpageModel>();
            A.CallTo(() => _webpageAdminService.GetAddModel(123)).Returns(addPageModel);

            var actionResult = _webpageController.Add_Get(123);

            actionResult.Model.Should().Be(addPageModel);
        }

        [Fact]
        public void WebpageController_AddGet_ShouldCallViewData()
        {
            var parent = new TextPage();
            var addPageModel = new AddWebpageModel { ParentId = 123 };
            A.CallTo(() => _webpageAdminService.GetAddModel(123)).Returns(addPageModel);
            A.CallTo(() => _webpageAdminService.GetWebpage(123)).Returns(parent);

            _webpageController.Add_Get(123);

            A.CallTo(() => _baseViewDataService.SetAddPageViewData(_webpageController.ViewData, parent))
                .MustHaveHappened();
        }

        [Fact]
        public async Task WebpageController_AddPost_ShouldCallAdd()
        {
            var model = new AddWebpageModel { UrlSegment = "test" };
            A.CallTo(() => _urlValidationService.UrlIsValidForWebpage("test", null)).Returns(true);
            var additionalModel = new object();
            A.CallTo(() => _webpageAdminService.GetAdditionalPropertyModel(model.DocumentType))
                .Returns(additionalModel);

            await _webpageController.Add(model);

            A.CallTo(() => _webpageAdminService.Add(model, additionalModel)).MustHaveHappened();
        }

        [Fact]
        public async Task WebpageController_AddPost_ShouldRedirectToEdit()
        {
            var model = new AddWebpageModel();
            A.CallTo(() => _urlValidationService.UrlIsValidForWebpage(null, null)).Returns(true);
            var value = new object();
            A.CallTo(() => _webpageAdminService.GetAdditionalPropertyModel(model.DocumentType)).Returns(value);
            A.CallTo(() => _webpageAdminService.Add(model,value)).Returns(new StubWebpage{Id=123});

            var result = (await _webpageController.Add(model)) as RedirectToActionResult;

            result.ActionName.Should().Be("Edit");
            result.RouteValues["id"].Should().Be(123); // from webpage returned by add service method
        }

        [Fact]
        public async Task WebpageController_AddPost_IfIsValidForWebpageIsFalseShouldReturnViewResult()
        {
            var model = new AddWebpageModel { };
            A.CallTo(() => _urlValidationService.UrlIsValidForWebpage(null, null)).Returns(false);

            var result = await _webpageController.Add(model);

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task WebpageController_AddPost_IfIsValidForWebpageIsFalseShouldReturnPassedObjectAsModel()
        {
            var model = new AddWebpageModel { };
            A.CallTo(() => _urlValidationService.UrlIsValidForWebpage(null, null)).Returns(false);

            var result = await _webpageController.Add(model);

            result.As<ViewResult>().Model.Should().Be(model);
        }

        [Fact]
        public void WebpageController_EditGet_ShouldReturnAViewResult()
        {
            var result = _webpageController.Edit_Get(123);

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void WebpageController_EditGet_ShouldReturnWebpageAsViewModel()
        {
            var webpage = new TextPage { Id = 1 };
            A.CallTo(() => _webpageAdminService.GetWebpage(123)).Returns(webpage);

            var result = _webpageController.Edit_Get(123);

            result.Model.Should().Be(webpage);
        }

        [Fact]
        public void WebpageController_EditGet_ShouldSetViewData()
        {
            var page = new TextPage();
            A.CallTo(() => _webpageAdminService.GetWebpage(123)).Returns(page);

            _webpageController.Edit_Get(123);

            A.CallTo(() => _baseViewDataService.SetEditPageViewData(_webpageController.ViewData, page))
                .MustHaveHappened();
        }

        [Fact]
        public void WebpageController_EditPost_ShouldCallSaveDocument()
        {
            A.CallTo(() => _urlValidationService.UrlIsValidForWebpage(null, 1)).Returns(true);
            var textPage = new UpdateWebpageViewModel { Id = 1 };

            _webpageController.Edit(textPage);

            A.CallTo(() => _webpageAdminService.Update(textPage)).MustHaveHappened();
        }

        [Fact]
        public void WebpageController_EditPost_ShouldRedirectToEdit()
        {
            A.CallTo(() => _urlValidationService.UrlIsValidForWebpage(null, 1)).Returns(true);
            var model = new UpdateWebpageViewModel();
            var stubWebpage = new StubWebpage{Id=123};
            A.CallTo(() => _webpageAdminService.Update(model)).Returns(stubWebpage);

            var actionResult = _webpageController.Edit(model);

            actionResult.ActionName.Should().Be("Edit");
            actionResult.RouteValues["id"].Should().Be(123);
        }

        [Fact]
        public void WebpageController_Sort_ShouldBeAListOfSortItems()
        {
            var parent = new TextPage();
            var sortItems = new List<SortItem>();
            A.CallTo(() => _webpageAdminService.GetSortItems(123)).Returns(sortItems);

            var viewResult = _webpageController.Sort(123).As<ViewResult>();

            viewResult.Model.Should().Be(sortItems);
        }

        [Fact]
        public void WebpageController_Index_ReturnsViewResult()
        {
            var actionResult = _webpageController.Index();

            actionResult.Should().NotBeNull();
        }

        [Fact]
        public void WebpageController_DeleteGet_ReturnsPartialViewResult()
        {
            _webpageController.Delete_Get(123).Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void WebpageController_DeleteGet_ReturnsWebpageAsModel()
        {
            var textPage = new TextPage();
            A.CallTo(() => _webpageAdminService.GetWebpage(123)).Returns(textPage);

            _webpageController.Delete_Get(123).As<PartialViewResult>().Model.Should().Be(textPage);
        }

        [Fact]
        public void WebpageController_Delete_ReturnsRedirectToIndex()
        {
            var actionResult = _webpageController.Delete(123);

            actionResult.ActionName.Should().Be("Index");
        }

        [Fact]
        public void WebpageController_Delete_CallsDeleteDocumentOnThePassedObject()
        {
            _webpageController.Delete(123);

            A.CallTo(() => _webpageAdminService.Delete(123)).MustHaveHappened();
        }

        [Fact]
        public void WebpageController_PublishNow_ReturnsRedirectToRouteResult()
        {
            _webpageController.PublishNow(123).Should().BeOfType<RedirectToActionResult>();
        }

        [Fact]
        public void WebpageController_PublishNow_RedirectsToEditForId()
        {
            var result = _webpageController.PublishNow(123);

            result.ActionName.Should().Be("Edit");
            result.RouteValues["id"].Should().Be(123);
        }

        [Fact]
        public void WebpageController_PublishNow_CallsPublishNow()
        {
            _webpageController.PublishNow(123);

            A.CallTo(() => _webpageAdminService.PublishNow(123)).MustHaveHappened();
        }

        [Fact]
        public void WebpageController_Unpublish_ReturnsRedirectToRouteResult()
        {
            _webpageController.Unpublish(123).Should().BeOfType<RedirectToActionResult>();
        }

        [Fact]
        public void WebpageController_Unpublish_RedirectsToEditForId()
        {
            var result = _webpageController.Unpublish(123);

            result.ActionName.Should().Be("Edit");
            result.RouteValues["id"].Should().Be(123);
        }

        [Fact]
        public void WebpageController_Unpublish_CallsUnpublishOnService()
        {
            _webpageController.Unpublish(123);

            A.CallTo(() => _webpageAdminService.Unpublish(123)).MustHaveHappened();
        }

        [Fact]
        public void WebpageController_ViewChanges_ShouldReturnPartialViewResult()
        {
            var documentVersion = new DocumentVersion();

            _webpageController.ViewChanges(documentVersion).Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void WebpageController_ViewChanges_NullDocumentVersionRedirectsToIndex()
        {
            var result = _webpageController.ViewChanges(null);

            result.Should().BeOfType<RedirectToActionResult>();
            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
        }
    }
}