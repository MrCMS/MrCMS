using System.Collections.Generic;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Multisite;
using MrCMS.Services;
using MrCMS.Web.Application.Pages;
using MrCMS.Web.Areas.Admin.Controllers;
using MrCMS.Web.Areas.Admin.Models;
using Xunit;

namespace MrCMS.Web.Tests.Areas.Admin.Controllers
{
    public class LayoutContollerTests
    {
        private ISiteService _siteService;
        private IDocumentService documentService;

        [Fact]
        public void LayoutController_AddGet_ShouldReturnAddPageModel()
        {
            LayoutController layoutController = GetLayoutController();

            var actionResult = layoutController.Add(1, 2) as ViewResult;

            actionResult.Model.Should().BeOfType<AddPageModel>();
        }

        private LayoutController GetLayoutController()
        {
            documentService = A.Fake<IDocumentService>();
            _siteService = A.Fake<ISiteService>();
            var layoutController = new LayoutController(documentService, _siteService);
            return layoutController;
        }

        [Fact]
        public void LayoutController_AddGet_ShouldSetParentIdOfModelToIdInMethod()
        {
            LayoutController layoutController = GetLayoutController();
            A.CallTo(() => documentService.GetDocument<Document>(1)).Returns(new TextPage {Id = 1, Site = new Site()});

            var actionResult = layoutController.Add(1, 2) as ViewResult;

            (actionResult.Model as AddPageModel).ParentId.Should().Be(1);
        }

        [Fact]
        public void LayoutController_AddPost_ShouldCallSaveDocument()
        {
            LayoutController layoutController = GetLayoutController();

            var layout = new Layout();
            layoutController.Add(layout);

            A.CallTo(() => documentService.AddDocument(layout)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void LayoutController_AddPost_ShouldRedirectToView()
        {
            LayoutController layoutController = GetLayoutController();

            var layout = new Layout {Id = 1};
            var result = layoutController.Add(layout) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("Edit");
            result.RouteValues["id"].Should().Be(1);
        }

        [Fact]
        public void LayoutController_EditGet_ShouldReturnAViewResult()
        {
            LayoutController layoutController = GetLayoutController();
            var layout = new Layout {Id = 1};

            ActionResult result = layoutController.Edit_Get(layout);

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void LayoutController_EditGet_ShouldReturnLayoutAsViewModel()
        {
            LayoutController layoutController = GetLayoutController();
            var layout = new Layout {Id = 1};

            var result = layoutController.Edit_Get(layout) as ViewResult;

            result.Model.Should().Be(layout);
        }

        [Fact]
        public void LayoutController_EditPost_ShouldCallSaveDocument()
        {
            LayoutController layoutController = GetLayoutController();
            var layout = new Layout {Id = 1};

            layoutController.Edit(layout);

            A.CallTo(() => documentService.SaveDocument(layout)).MustHaveHappened();
        }

        [Fact]
        public void LayoutController_EditPost_ShouldRedirectToEdit()
        {
            LayoutController layoutController = GetLayoutController();
            var layout = new Layout {Id = 1};

            ActionResult actionResult = layoutController.Edit(layout);

            actionResult.Should().BeOfType<RedirectToRouteResult>();
            (actionResult as RedirectToRouteResult).RouteValues["action"].Should().Be("Edit");
            (actionResult as RedirectToRouteResult).RouteValues["id"].Should().Be(1);
        }

        [Fact]
        public void LayoutController_Sort_ShouldCallGetDocumentsByParentId()
        {
            LayoutController layoutController = GetLayoutController();

            layoutController.Sort(1);

            A.CallTo(() => documentService.GetDocumentsByParentId<Layout>(1)).MustHaveHappened();
        }

        [Fact]
        public void LayoutController_Sort_ShouldUseTheResultOfDocumentsByParentIdsAsModel()
        {
            LayoutController layoutController = GetLayoutController();
            var layouts = new List<Layout> {new Layout()};
            A.CallTo(() => documentService.GetDocumentsByParentId<Layout>(1)).Returns(layouts);

            var viewResult = layoutController.Sort(1).As<ViewResult>();

            viewResult.Model.As<List<Layout>>().Should().BeEquivalentTo(layouts);
        }

        [Fact]
        public void LayoutController_SortAction_ShouldCallSortOrderOnTheDocumentServiceWithTheRelevantValues()
        {
            LayoutController layoutController = GetLayoutController();
            layoutController.SortAction(1, 2);

            A.CallTo(() => documentService.SetOrder(1, 2)).MustHaveHappened();
        }

        [Fact]
        public void LayoutController_View_InvalidIdReturnsRedirectToIndex()
        {
            LayoutController layoutController = GetLayoutController();

            ActionResult actionResult = layoutController.Show(null);

            actionResult.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void LayoutController_Index_ReturnsViewResult()
        {
            LayoutController layoutController = GetLayoutController();

            ViewResult actionResult = layoutController.Index();

            actionResult.Should().NotBeNull();
        }
    }
}