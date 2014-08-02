using System.Collections.Generic;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Controllers;
using Xunit;

namespace MrCMS.Web.Tests.Areas.Admin.Controllers
{
    public class LayoutContollerTests
    {
        private readonly IDocumentService _documentService;
        private readonly LayoutController _layoutController;
        private readonly IUrlValidationService _urlValidationService;

        public LayoutContollerTests()
        {
            _documentService = A.Fake<IDocumentService>();
            _urlValidationService = A.Fake<IUrlValidationService>();
            _layoutController = new LayoutController(_documentService, _urlValidationService);
        }

        [Fact]
        public void LayoutController_AddGet_ShouldReturnANewLayoutObject()
        {
            var actionResult = _layoutController.Add_Get(null) as ViewResult;

            actionResult.Model.Should().BeOfType<Layout>();
        }

        [Fact]
        public void LayoutController_AddGet_ShouldSetParentOfModelToParentInMethod()
        {
            var parent = new Layout {Id = 1};
            A.CallTo(() => _documentService.GetDocument<Layout>(1)).Returns(parent);

            var actionResult = _layoutController.Add_Get(1) as ViewResult;

            actionResult.Model.As<Layout>().Parent.Should().Be(parent);
        }

        [Fact]
        public void LayoutController_AddPost_ShouldCallSaveDocument()
        {
            var layout = new Layout();

            _layoutController.Add(layout);

            A.CallTo(() => _documentService.AddDocument(layout)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void LayoutController_AddPost_ShouldRedirectToView()
        {
            var layout = new Layout {Id = 1};

            var result = _layoutController.Add(layout) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("Edit");
            result.RouteValues["id"].Should().Be(1);
        }

        [Fact]
        public void LayoutController_EditGet_ShouldReturnAViewResult()
        {
            var layout = new Layout {Id = 1};

            ActionResult result = _layoutController.Edit_Get(layout);

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void LayoutController_EditGet_ShouldReturnLayoutAsViewModel()
        {
            var layout = new Layout {Id = 1};

            var result = _layoutController.Edit_Get(layout) as ViewResult;

            result.Model.Should().Be(layout);
        }

        [Fact]
        public void LayoutController_EditPost_ShouldCallSaveDocument()
        {
            var layout = new Layout {Id = 1};

            _layoutController.Edit(layout);

            A.CallTo(() => _documentService.SaveDocument(layout)).MustHaveHappened();
        }

        [Fact]
        public void LayoutController_EditPost_ShouldRedirectToEdit()
        {
            var layout = new Layout {Id = 1};

            ActionResult actionResult = _layoutController.Edit(layout);

            actionResult.Should().BeOfType<RedirectToRouteResult>();
            (actionResult as RedirectToRouteResult).RouteValues["action"].Should().Be("Edit");
            (actionResult as RedirectToRouteResult).RouteValues["id"].Should().Be(1);
        }

        [Fact]
        public void LayoutController_Sort_ShouldCallGetDocumentsByParentId()
        {
            var parent = new Layout();

            _layoutController.Sort(parent);

            A.CallTo(() => _documentService.GetDocumentsByParent(parent)).MustHaveHappened();
        }

        [Fact]
        public void LayoutController_Sort_ShouldBeAListOfSortItems()
        {
            var layout = new Layout();
            var layouts = new List<Layout> {new Layout()};
            A.CallTo(() => _documentService.GetDocumentsByParent(layout)).Returns(layouts);

            var viewResult = _layoutController.Sort(layout).As<ViewResult>();

            viewResult.Model.Should().BeOfType<List<SortItem>>();
        }

        [Fact]
        public void LayoutController_View_InvalidIdReturnsRedirectToIndex()
        {
            ActionResult actionResult = _layoutController.Show(null);

            actionResult.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void LayoutController_Index_ReturnsViewResult()
        {
            ViewResult actionResult = _layoutController.Index();

            actionResult.Should().NotBeNull();
        }
    }
}