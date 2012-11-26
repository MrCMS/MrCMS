using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Controllers;
using MrCMS.Web.Areas.Admin.Models;
using Xunit;

namespace MrCMS.Web.Tests.Areas.Admin.Controllers
{
    public class WebpageControllerTests
    {
        private static IDocumentService documentService;

        [Fact]
        public void WebpageController_AddGet_ShouldReturnAddPageModel()
        {
            var webpageController = GetWebpageController();

            var actionResult = webpageController.Add(1) as ViewResult;

            actionResult.Model.Should().BeOfType<AddPageModel>();
        }

        private static WebpageController GetWebpageController()
        {
            documentService = A.Fake<IDocumentService>();
            var formService = A.Fake<IFormService>();
            var webpageController = new WebpageController(documentService,formService) {IsAjaxRequest = false};
            return webpageController;
        }

        [Fact]
        public void WebpageController_AddGet_ShouldSetParentIdOfModelToIdInMethod()
        {
            var webpageController = GetWebpageController();
            A.CallTo(() => documentService.GetDocument<Document>(1)).Returns(new TextPage {Id = 1});

            var actionResult = webpageController.Add(1) as ViewResult;

            (actionResult.Model as AddPageModel).ParentId.Should().Be(1);
        }

        [Fact]
        public void WebpageController_AddGet_ShouldSetViewDataToSelectListItem()
        {
            var webpageController = GetWebpageController();

            var result = webpageController.Add(1) as ViewResult;

            webpageController.ViewData["Layout"].Should().BeAssignableTo<IEnumerable<SelectListItem>>();
        }

        [Fact]
        public void WebpageController_AddPost_ShouldCallSaveDocument()
        {
            var webpageController = GetWebpageController();

            var webpage = new TextPage();
            webpageController.Add(webpage);

            A.CallTo(() => documentService.AddDocument(webpage)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void WebpageController_AddPost_ShouldRedirectToView()
        {
            var webpageController = GetWebpageController();

            var webpage = new TextPage { Id = 1 };
            var result = webpageController.Add(webpage) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("Edit");
            result.RouteValues["id"].Should().Be(1);
        }

        [Fact]
        public void WebpageController_EditGet_ShouldReturnAViewResult()
        {
            var webpageController = GetWebpageController();

            ActionResult result = webpageController.Edit(1);

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void WebpageController_EditGet_ShouldReturnLayoutAsViewModel()
        {
            var webpageController = GetWebpageController();
            var webpage = new TextPage { Id = 1 };
            A.CallTo(() => documentService.GetDocument<Webpage>(1)).Returns(webpage);

            var result = webpageController.Edit(1) as ViewResult;

            result.Model.Should().Be(webpage);
        }

        [Fact]
        public void WebpageController_EditGet_ShouldCallGetAllLayouts()
        {
            var webpageController = GetWebpageController();

            webpageController.Edit(1);

            A.CallTo(() => documentService.GetAllDocuments<Layout>()).MustHaveHappened();
        }

        [Fact]
        public void WebpageController_EditGet_ShouldSetViewDataToSelectListItem()
        {
            var webpageController = GetWebpageController();

            var result = webpageController.Edit(1) as ViewResult;

            webpageController.ViewData["Layout"].Should().BeAssignableTo<IEnumerable<SelectListItem>>();
        }

        [Fact]
        public void WebpageController_EditGet_ShouldSetLayoutDetailsToSelectListItems()
        {
            var webpageController = GetWebpageController();
            var layout = new Layout() { Id = 1, Name = "Layout Name" };
            A.CallTo(() => documentService.GetAllDocuments<Layout>()).Returns(new List<Layout> { layout });

            webpageController.Edit(1);

            webpageController.ViewData["Layout"].As<IEnumerable<SelectListItem>>().Skip(1).First().Selected.Should().BeFalse();
            webpageController.ViewData["Layout"].As<IEnumerable<SelectListItem>>().Skip(1).First().Text.Should().Be("Layout Name");
            webpageController.ViewData["Layout"].As<IEnumerable<SelectListItem>>().Skip(1).First().Value.Should().Be("1");
        }

        [Fact]
        public void WebpageController_EditPost_ShouldCallSaveDocument()
        {
            var webpageController = GetWebpageController();
            Webpage textPage = new TextPage { Id = 1 };

            webpageController.Edit(textPage);

            A.CallTo(() => documentService.SaveDocument(textPage)).MustHaveHappened();
        }

        [Fact]
        public void WebpageController_EditPost_ShouldRedirectToEdit()
        {
            var webpageController = GetWebpageController();
            var textPage = new TextPage { Id = 1 };

            ActionResult actionResult = webpageController.Edit(textPage);

            actionResult.Should().BeOfType<RedirectToRouteResult>();
            actionResult.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Edit");
            actionResult.As<RedirectToRouteResult>().RouteValues["id"].Should().Be(1);
        }

        [Fact]
        public void WebpageController_Sort_ShouldCallGetDocumentsByParentId()
        {
            var webpageController = GetWebpageController();

            webpageController.Sort(1);

            A.CallTo(() => documentService.GetAdminDocumentsByParentId<Webpage>(1)).MustHaveHappened();
        }

        [Fact]
        public void WebpageController_Sort_ShouldUseTheResultOfDocumentsByParentIdsAsModel()
        {
            var webpageController = GetWebpageController();
            var webpages = new List<Webpage> { new TextPage() };
            A.CallTo(() => documentService.GetAdminDocumentsByParentId<Webpage>(1)).Returns(webpages);

            var viewResult = webpageController.Sort(1).As<ViewResult>();

            viewResult.Model.As<List<Webpage>>().Should().BeEquivalentTo(webpages);
        }

        [Fact]
        public void WebpageController_SortAction_ShouldCallSortOrderOnTheDocumentServiceWithTheRelevantValues()
        {
            var webpageController = GetWebpageController();
            webpageController.SortAction(1, 2);

            A.CallTo(() => documentService.SetOrder(1, 2)).MustHaveHappened();
        }

        [Fact]
        public void WebpageController_View_CallsGetDocumentWithId()
        {
            var webpageController = GetWebpageController();

            webpageController.View(1);

            A.CallTo(() => documentService.GetDocument<Webpage>(1)).MustHaveHappened();
        }

        [Fact]
        public void WebpageController_View_InvalidIdReturnsRedirectToIndex()
        {
            var webpageController = GetWebpageController();
            A.CallTo(() => documentService.GetDocument<Webpage>(1)).Returns(null);

            var actionResult = webpageController.View(1);

            actionResult.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void WebpageController_Index_ReturnsViewResult()
        {
            var webpageController = GetWebpageController();

            var actionResult = webpageController.Index();

            actionResult.Should().NotBeNull();
        }

        [Fact]
        public void WebpageController_SuggestDocumentUrl_ShouldCallGetDocumentUrl()
        {
            var webpageController = GetWebpageController();

            webpageController.SuggestDocumentUrl(1, "test");

            A.CallTo(() => documentService.GetDocumentUrl("test", 1, false)).MustHaveHappened();
        }

        [Fact]
        public void WebpageController_SuggestDocumentUrl_ShouldReturnTheResultOfGetDocumentUrl()
        {
            var webpageController = GetWebpageController();

            A.CallTo(() => documentService.GetDocumentUrl("test", 1, false)).Returns("test/result");
            var url = webpageController.SuggestDocumentUrl(1, "test");

            url.Should().BeEquivalentTo("test/result");
        }
    }
}