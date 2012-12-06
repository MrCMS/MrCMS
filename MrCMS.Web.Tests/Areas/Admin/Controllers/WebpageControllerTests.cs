using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Models;
using MrCMS.Paging;
using MrCMS.Services;
using MrCMS.Tests.Entities;
using MrCMS.Tests.Stubs;
using MrCMS.Web.Areas.Admin.Controllers;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Website;
using Xunit;
using MrCMS.Helpers;

namespace MrCMS.Web.Tests.Areas.Admin.Controllers
{
    public class WebpageControllerTests
    {
        private static IDocumentService documentService;
        private static IFormService formService;
        private static ISitesService sitesService;

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
            sitesService = A.Fake<ISitesService>();
            formService = A.Fake<IFormService>();
            var webpageController = new WebpageController(documentService, sitesService, formService) { IsAjaxRequest = false };
            return webpageController;
        }

        [Fact]
        public void WebpageController_AddGet_ShouldSetParentIdOfModelToIdInMethod()
        {
            var webpageController = GetWebpageController();
            A.CallTo(() => documentService.GetDocument<Document>(1)).Returns(new TextPage { Id = 1 });

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

            ActionResult result = webpageController.Edit_Get(new TextPage());

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void WebpageController_EditGet_ShouldReturnLayoutAsViewModel()
        {
            var webpageController = GetWebpageController();
            var webpage = new TextPage { Id = 1 };

            var result = webpageController.Edit_Get(webpage) as ViewResult;

            result.Model.Should().Be(webpage);
        }

        [Fact]
        public void WebpageController_EditGet_ShouldCallGetAllLayouts()
        {
            var webpageController = GetWebpageController();

            webpageController.Edit_Get(new TextPage());

            A.CallTo(() => documentService.GetAllDocuments<Layout>()).MustHaveHappened();
        }

        [Fact]
        public void WebpageController_EditGet_ShouldSetViewDataToSelectListItem()
        {
            var webpageController = GetWebpageController();

            var result = webpageController.Edit_Get(new TextPage()) as ViewResult;

            webpageController.ViewData["Layout"].Should().BeAssignableTo<IEnumerable<SelectListItem>>();
        }

        [Fact]
        public void WebpageController_EditGet_ShouldSetLayoutDetailsToSelectListItems()
        {
            var webpageController = GetWebpageController();
            var layout = new Layout() { Id = 1, Name = "Layout Name" };
            A.CallTo(() => documentService.GetAllDocuments<Layout>()).Returns(new List<Layout> { layout });

            webpageController.Edit_Get(new TextPage());

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

            A.CallTo(() => documentService.GetDocumentsByParentId<Webpage>(1)).MustHaveHappened();
        }

        [Fact]
        public void WebpageController_Sort_ShouldUseTheResultOfDocumentsByParentIdsAsModel()
        {
            var webpageController = GetWebpageController();
            var webpages = new List<Webpage> { new TextPage() };
            A.CallTo(() => documentService.GetDocumentsByParentId<Webpage>(1)).Returns(webpages);

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
        public void WebpageController_View_InvalidIdReturnsRedirectToIndex()
        {
            var webpageController = GetWebpageController();
            A.CallTo(() => documentService.GetDocument<Webpage>(1)).Returns(null);

            var actionResult = webpageController.Show(null);

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

        [Fact]
        public void WebpageController_DeleteGet_ReturnsPartialViewResult()
        {
            var webpageController = GetWebpageController();

            webpageController.Delete_Get(null).Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void WebpageController_DeleteGet_ReturnsDocumentPassedAsModel()
        {
            var webpageController = GetWebpageController();

            var textPage = new TextPage();
            webpageController.Delete_Get(textPage).As<PartialViewResult>().Model.Should().Be(textPage);
        }

        [Fact]
        public void WebpageController_Delete_ReturnsRedirectToIndex()
        {
            var webpageController = GetWebpageController();

            webpageController.Delete(null).Should().BeOfType<RedirectToRouteResult>();
            webpageController.Delete(null).As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void WebpageController_Delete_CallsDeleteDocumentOnThePassedObject()
        {
            var webpageController = GetWebpageController();

            var textPage = new TextPage();
            webpageController.Delete(textPage);

            A.CallTo(() => documentService.DeleteDocument<Webpage>(textPage)).MustHaveHappened();
        }

        [Fact]
        public void WebpageController_PublishNow_ReturnsRedirectToRouteResult()
        {
            var webpageController = GetWebpageController();

            var textPage = new TextPage { Id = 1 };
            webpageController.PublishNow(textPage).Should().BeOfType<RedirectToRouteResult>();
        }

        [Fact]
        public void WebpageController_PublishNow_RedirectsToEditForId()
        {
            var webpageController = GetWebpageController();

            var textPage = new TextPage { Id = 1 };
            var result = webpageController.PublishNow(textPage).As<RedirectToRouteResult>();

            result.RouteValues["action"].Should().Be("Edit");
            result.RouteValues["id"].Should().Be(1);
        }

        [Fact]
        public void WebpageController_PublishNow_CallsDocumentServicePublishNow()
        {
            var webpageController = GetWebpageController();

            var textPage = new TextPage();
            webpageController.PublishNow(textPage);

            A.CallTo(() => documentService.PublishNow(textPage)).MustHaveHappened();
        }

        [Fact]
        public void WebpageController_Unpublish_ReturnsRedirectToRouteResult()
        {
            var webpageController = GetWebpageController();

            var textPage = new TextPage { Id = 1 };
            webpageController.Unpublish(textPage).Should().BeOfType<RedirectToRouteResult>();
        }

        [Fact]
        public void WebpageController_Unpublish_RedirectsToEditForId()
        {
            var webpageController = GetWebpageController();

            var textPage = new TextPage { Id = 1 };
            var result = webpageController.Unpublish(textPage).As<RedirectToRouteResult>();

            result.RouteValues["action"].Should().Be("Edit");
            result.RouteValues["id"].Should().Be(1);
        }

        [Fact]
        public void WebpageController_Unpublish_CallsDocumentServicePublishNow()
        {
            var webpageController = GetWebpageController();

            var textPage = new TextPage();
            webpageController.Unpublish(textPage);

            A.CallTo(() => documentService.Unpublish(textPage)).MustHaveHappened();
        }

        [Fact]
        public void WebpageController_ViewChanges_ShouldReturnPartialViewResult()
        {
            var webpageController = GetWebpageController();

            webpageController.ViewChanges(1).Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void WebpageController_ViewChanges_ShouldCallDocumentServiceGetDocumentVersion()
        {
            var webpageController = GetWebpageController();

            webpageController.ViewChanges(1);

            A.CallTo(() => documentService.GetDocumentVersion(1)).MustHaveHappened();
        }

        [Fact]
        public void WebpageController_ViewChanges_NullDocumentVersionRedirectsToIndex()
        {
            var webpageController = GetWebpageController();

            A.CallTo(() => documentService.GetDocumentVersion(1)).Returns(null);

            var result = webpageController.ViewChanges(1);

            result.Should().BeOfType<RedirectToRouteResult>();
            result.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void WebpageController_GetForm_ShouldReturnAJsonNetResult()
        {
            var webpageController = GetWebpageController();

            webpageController.GetForm(new TextPage()).Should().BeOfType<AdminController.JsonNetResult>();
        }

        [Fact]
        public void WebpageController_GetForm_ShouldCallFormServiceGetFormStructure()
        {
            var webpageController = GetWebpageController();

            var textPage = new TextPage();
            webpageController.GetForm(textPage);

            A.CallTo(() => formService.GetFormStructure(textPage)).MustHaveHappened();
        }

        [Fact]
        public void WebpageController_SaveForm_ShouldCallFormServiceSaveFormStructure()
        {
            var webpageController = GetWebpageController();

            webpageController.SaveForm(1, "data");

            A.CallTo(() => formService.SaveFormStructure(1, "data")).MustHaveHappened();
        }

        [Fact]
        public void WebpageController_GetUnformattedBodyContent_ReturnsBodyContentOfPassedTextPage()
        {
            var webpageController = GetWebpageController();

            var textpage = new TextPage { BodyContent = "test body content" };
            var unformattedBodyContent = webpageController.GetUnformattedBodyContent(textpage);

            unformattedBodyContent.Should().Be("test body content");
        }

        [Fact]
        public void WebpageController_ViewPosting_ShouldReturnAPartialViewResult()
        {
            var webpageController = GetWebpageController();

            webpageController.ViewPosting(1).Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void WebpageController_ViewPosting_ShouldCallGetFormPostingForPassedId()
        {
            var webpageController = GetWebpageController();

            webpageController.ViewPosting(1);

            A.CallTo(() => formService.GetFormPosting(1)).MustHaveHappened();
        }

        [Fact]
        public void WebpageController_ViewPosting_ReturnsTheResultOfTheCallToGetFormPostingAsTheModel()
        {
            var webpageController = GetWebpageController();

            var formPosting = new FormPosting();
            A.CallTo(() => formService.GetFormPosting(1)).Returns(formPosting);
            webpageController.ViewPosting(1).As<PartialViewResult>().Model.Should().Be(formPosting);
        }

        [Fact]
        public void WebpageController_SetParent_CallsDocumentServiceSetParentWithPassedArguments()
        {
            var webpageController = GetWebpageController();

            var textPage = new TextPage();
            webpageController.SetParent(textPage, 2);

            A.CallTo(() => documentService.SetParent(textPage, 2));
        }

        [Fact]
        public void WebpageController_Postings_ReturnsAPartialViewResult()
        {
            var webpageController = GetWebpageController();

            webpageController.Postings(new TextPage(), 1, null).Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void WebpageController_Postings_CallsFormServiceGetFormPostingsWithPassedArguments()
        {
            var webpageController = GetWebpageController();

            var textPage = new TextPage();
            webpageController.Postings(textPage, 1, null);

            A.CallTo(() => formService.GetFormPostings(textPage, 1, null)).MustHaveHappened();
        }

        [Fact]
        public void WebpageController_Posting_ReturnsTheResultOfTheCallToGetFormPostings()
        {
            var webpageController = GetWebpageController();

            var textPage = new TextPage();
            var postingsModel = new PostingsModel(new PagedList<FormPosting>(new FormPosting[0], 1, 1), 1);
            A.CallTo(() => formService.GetFormPostings(textPage, 1, null)).Returns(postingsModel);
            webpageController.Postings(textPage, 1, null).As<PartialViewResult>().Model.Should().Be(postingsModel);
        }

        [Fact]
        public void WebpageController_Versions_ReturnsPartialViewResult()
        {
            var webpageController = GetWebpageController();

            var document = new StubDocument();
            document.SetVersions(new List<DocumentVersion>());
            webpageController.Versions(document, 1).Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void WebpageController_Versions_CallsGetVersionsOnPassedDocument()
        {
            var webpageController = GetWebpageController();

            var document = A.Fake<StubDocument>();
            document.SetVersions(new List<DocumentVersion>());
            webpageController.Versions(document, 1);

            A.CallTo(() => document.GetVersions(1)).MustHaveHappened();
        }


        [Fact]
        public void WebpageController_HideWidget_CallsDocumentServiceHideWidgetWithPassedArguments()
        {
            var webpageController = GetWebpageController();

            webpageController.HideWidget(1, 2, 3);

            A.CallTo(() => documentService.HideWidget(1, 2)).MustHaveHappened();
        }

        [Fact]
        public void WebpageController_HideWidget_ReturnsARedirectToRouteResult()
        {
            var webpageController = GetWebpageController();

            webpageController.HideWidget(1, 2, 3).Should().BeOfType<RedirectToRouteResult>();
        }

        [Fact]
        public void WebpageController_HideWidget_SetsRouteValuesForIdAndLayoutAreaId()
        {
            var webpageController = GetWebpageController();

            var redirectToRouteResult = webpageController.HideWidget(1, 2, 3).As<RedirectToRouteResult>();

            redirectToRouteResult.RouteValues["action"].Should().Be("Edit");
            redirectToRouteResult.RouteValues["id"].Should().Be(1);
            redirectToRouteResult.RouteValues["layoutAreaId"].Should().Be(3);
        }

        [Fact]
        public void WebpageController_ShowWidget_CallsDocumentServiceShowWidgetWithPassedArguments()
        {
            var webpageController = GetWebpageController();

            webpageController.ShowWidget(1, 2, 3);

            A.CallTo(() => documentService.ShowWidget(1, 2)).MustHaveHappened();
        }

        [Fact]
        public void WebpageController_ShowWidget_ReturnsARedirectToRouteResult()
        {
            var webpageController = GetWebpageController();

            webpageController.ShowWidget(1, 2, 3).Should().BeOfType<RedirectToRouteResult>();
        }

        [Fact]
        public void WebpageController_ShowWidget_SetsRouteValuesForIdAndLayoutAreaId()
        {
            var webpageController = GetWebpageController();

            var redirectToRouteResult = webpageController.ShowWidget(1, 2, 3).As<RedirectToRouteResult>();

            redirectToRouteResult.RouteValues["action"].Should().Be("Edit");
            redirectToRouteResult.RouteValues["id"].Should().Be(1);
            redirectToRouteResult.RouteValues["layoutAreaId"].Should().Be(3);
        }

    }
}