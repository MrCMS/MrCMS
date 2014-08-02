using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.People;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Web.Areas.Admin.Controllers;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Web.Tests.Stubs;
using MrCMS.Website;
using Xunit;

namespace MrCMS.Web.Tests.Areas.Admin.Controllers
{
    public class WebpageControllerTests : MrCMSTest
    {
        private readonly IDocumentService _documentService;
        private readonly IUrlValidationService _urlValidationService;
        private readonly IWebpageBaseViewDataService _baseViewDataService;
        private readonly WebpageController _webpageController;

        public WebpageControllerTests()
        {
            Kernel.Bind<ISetWebpageAdminViewData>().ToConstant(A.Fake<ISetWebpageAdminViewData>());
            CurrentRequestData.CurrentUser = new User();
            _documentService = A.Fake<IDocumentService>();
            _urlValidationService = A.Fake<IUrlValidationService>();
            _baseViewDataService = A.Fake<IWebpageBaseViewDataService>();
            _webpageController = new WebpageController(_baseViewDataService, _documentService,
                _urlValidationService)
            {
                RouteDataMock = new RouteData()
            };
        }

        [Fact]
        public void WebpageController_AddGet_ShouldReturnAddPageModel()
        {
            var actionResult = _webpageController.Add_Get(1) as ViewResult;

            actionResult.Model.Should().BeOfType<AddPageModel>();
        }

        [Fact]
        public void WebpageController_AddGet_ShouldSetParentIdOfModelToIdInMethod()
        {
            var textPage = new TextPage { Id = 1};
            A.CallTo(() => _documentService.GetDocument<Webpage>(1)).Returns(textPage);

            var actionResult = _webpageController.Add_Get(1) as ViewResult;

            (actionResult.Model as AddPageModel).ParentId.Should().Be(1);
        }

        [Fact]
        public void WebpageController_AddGet_ShouldCallViewData()
        {
            var textPage = new TextPage {};
            A.CallTo(() => _documentService.GetDocument<Webpage>(1)).Returns(textPage);

            _webpageController.Add_Get(1);

            A.CallTo(() => _baseViewDataService.SetAddPageViewData(_webpageController.ViewData, textPage))
                .MustHaveHappened();
        }

        [Fact]
        public void WebpageController_AddPost_ShouldCallSaveDocument()
        {
            var webpage = new TextPage {};
            A.CallTo(() => _urlValidationService.UrlIsValidForWebpage(null, null)).Returns(true);

            _webpageController.Add(webpage);

            A.CallTo(() => _documentService.AddDocument<Webpage>(webpage)).MustHaveHappened();
        }

        [Fact]
        public void WebpageController_AddPost_ShouldRedirectToEdit()
        {
            var webpage = new TextPage {Id = 1};
            A.CallTo(() => _urlValidationService.UrlIsValidForWebpage(null, null)).Returns(true);

            var result = _webpageController.Add(webpage) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("Edit");
            result.RouteValues["id"].Should().Be(1);
        }

        [Fact]
        public void WebpageController_AddPost_IfIsValidForWebpageIsFalseShouldReturnViewResult()
        {
            var webpage = new TextPage {Id = 1};
            A.CallTo(() => _urlValidationService.UrlIsValidForWebpage(null, null)).Returns(false);

            ActionResult result = _webpageController.Add(webpage);

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void WebpageController_AddPost_IfIsValidForWebpageIsFalseShouldReturnPassedObjectAsModel()
        {
            var webpage = new TextPage {Id = 1};
            A.CallTo(() => _urlValidationService.UrlIsValidForWebpage(null, null)).Returns(false);

            ActionResult result = _webpageController.Add(webpage);

            result.As<ViewResult>().Model.Should().Be(webpage);
        }

        [Fact]
        public void WebpageController_EditGet_ShouldReturnAViewResult()
        {
            ActionResult result = _webpageController.Edit_Get(new TextPage());

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void WebpageController_EditGet_ShouldReturnLayoutAsViewModel()
        {
            var webpage = new TextPage {Id = 1};

            var result = _webpageController.Edit_Get(webpage) as ViewResult;

            result.Model.Should().Be(webpage);
        }

        [Fact]
        public void WebpageController_EditGet_ShouldSetViewData()
        {
            var textPage = new TextPage();

            _webpageController.Edit_Get(textPage);

            A.CallTo(() => _baseViewDataService.SetEditPageViewData(_webpageController.ViewData, textPage))
                .MustHaveHappened();
        }

        [Fact]
        public void WebpageController_EditPost_ShouldCallSaveDocument()
        {
            A.CallTo(() => _urlValidationService.UrlIsValidForWebpage(null, 1)).Returns(true);
            Webpage textPage = new TextPage {Id = 1};

            _webpageController.Edit(textPage);

            A.CallTo(() => _documentService.SaveDocument(textPage)).MustHaveHappened();
        }

        [Fact]
        public void WebpageController_EditPost_ShouldRedirectToEdit()
        {
            A.CallTo(() => _urlValidationService.UrlIsValidForWebpage(null, 1)).Returns(true);
            var textPage = new TextPage {Id = 1};

            ActionResult actionResult = _webpageController.Edit(textPage);

            actionResult.Should().BeOfType<RedirectToRouteResult>();
            actionResult.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Edit");
            actionResult.As<RedirectToRouteResult>().RouteValues["id"].Should().Be(1);
        }

        [Fact]
        public void WebpageController_Sort_ShouldCallGetDocumentsByParentId()
        {
            var textPage = new TextPage();

            _webpageController.Sort(textPage);

            A.CallTo(() => _documentService.GetDocumentsByParent<Webpage>(textPage)).MustHaveHappened();
        }

        [Fact]
        public void WebpageController_Sort_ShouldBeAListOfSortItems()
        {
            var textPage = new TextPage();
            var webpages = new List<Webpage> {new TextPage()};
            A.CallTo(() => _documentService.GetDocumentsByParent<Webpage>(textPage)).Returns(webpages);

            var viewResult = _webpageController.Sort(textPage).As<ViewResult>();

            viewResult.Model.Should().BeOfType<List<SortItem>>();
        }

        [Fact]
        public void WebpageController_View_InvalidIdReturnsRedirectToIndex()
        {
            A.CallTo(() => _documentService.GetDocument<Webpage>(1)).Returns(null);

            ActionResult actionResult = _webpageController.Show(null);

            actionResult.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void WebpageController_Index_ReturnsViewResult()
        {
            ViewResult actionResult = _webpageController.Index();

            actionResult.Should().NotBeNull();
        }

        [Fact]
        public void WebpageController_DeleteGet_ReturnsPartialViewResult()
        {
            _webpageController.Delete_Get(null).Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void WebpageController_DeleteGet_ReturnsDocumentPassedAsModel()
        {
            var textPage = new TextPage();

            _webpageController.Delete_Get(textPage).As<PartialViewResult>().Model.Should().Be(textPage);
        }

        [Fact]
        public void WebpageController_Delete_ReturnsRedirectToIndex()
        {
            var stubWebpage = new StubWebpage();

            _webpageController.Delete(stubWebpage).Should().BeOfType<RedirectToRouteResult>();
            _webpageController.Delete(stubWebpage).As<RedirectToRouteResult>().RouteValues["action"].Should()
                .Be("Index");
        }

        [Fact]
        public void WebpageController_Delete_CallsDeleteDocumentOnThePassedObject()
        {
            var textPage = new TextPage();
            _webpageController.Delete(textPage);

            A.CallTo(() => _documentService.DeleteDocument<Webpage>(textPage)).MustHaveHappened();
        }

        [Fact]
        public void WebpageController_PublishNow_ReturnsRedirectToRouteResult()
        {
            var textPage = new TextPage {Id = 1};
            _webpageController.PublishNow(textPage).Should().BeOfType<RedirectToRouteResult>();
        }

        [Fact]
        public void WebpageController_PublishNow_RedirectsToEditForId()
        {
            var textPage = new TextPage {Id = 1};
            var result = _webpageController.PublishNow(textPage).As<RedirectToRouteResult>();

            result.RouteValues["action"].Should().Be("Edit");
            result.RouteValues["id"].Should().Be(1);
        }

        [Fact]
        public void WebpageController_PublishNow_CallsDocumentServicePublishNow()
        {
            var textPage = new TextPage();

            _webpageController.PublishNow(textPage);

            A.CallTo(() => _documentService.PublishNow(textPage)).MustHaveHappened();
        }

        [Fact]
        public void WebpageController_Unpublish_ReturnsRedirectToRouteResult()
        {
            var textPage = new TextPage {Id = 1};
            _webpageController.Unpublish(textPage).Should().BeOfType<RedirectToRouteResult>();
        }

        [Fact]
        public void WebpageController_Unpublish_RedirectsToEditForId()
        {
            var textPage = new TextPage {Id = 1};
            var result = _webpageController.Unpublish(textPage).As<RedirectToRouteResult>();

            result.RouteValues["action"].Should().Be("Edit");
            result.RouteValues["id"].Should().Be(1);
        }

        [Fact]
        public void WebpageController_Unpublish_CallsDocumentServicePublishNow()
        {
            var textPage = new TextPage();
            _webpageController.Unpublish(textPage);

            A.CallTo(() => _documentService.Unpublish(textPage)).MustHaveHappened();
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
            ActionResult result = _webpageController.ViewChanges(null);

            result.Should().BeOfType<RedirectToRouteResult>();
            result.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Index");
        }
    }
}