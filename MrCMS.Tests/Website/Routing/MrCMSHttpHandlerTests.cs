using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.People;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Tests.Stubs;
using MrCMS.Website;
using MrCMS.Website.Routing;
using Xunit;

namespace MrCMS.Tests.Website.Routing
{
    public class MrCMSHttpHandlerTests
    {
        public MrCMSHttpHandlerTests()
        {
            MrCMSApplication.DatabaseIsInstalled = true;
            MrCMSApplication.OverriddenUser = new User();
            MrCMSApplication.OverridenContext = A.Fake<HttpContextBase>();
            MrCMSApplication.OverridenRootChildren = new List<Webpage>();
        }
        [Fact]
        public void MrCMSHttpHandler_CheckIsInstalled_DatabaseIsNotInstalledRedirectsToInstall()
        {
            MrCMSApplication.DatabaseIsInstalled = false;
            var mrCMSHttpHandler = new MrCMSHttpHandler(A.Fake<RequestContext>(), null, null);
            var httpContext = A.Fake<HttpContextBase>();

            mrCMSHttpHandler.CheckIsInstalled(httpContext).Should().BeFalse();

            A.CallTo(() => httpContext.Response.Redirect("~/Install")).MustHaveHappened();
        }

        [Fact]
        public void MrCMSHttpHandler_CheckIsInstalled_DatabaseIsInstalledReturnsTrue()
        {
            var mrCMSHttpHandler = new MrCMSHttpHandler(A.Fake<RequestContext>(), null, null);
            var httpContext = A.Fake<HttpContextBase>();

            mrCMSHttpHandler.CheckIsInstalled(httpContext).Should().BeTrue();
        }

        [Fact]
        public void MrCMSHttpHandler_IsReusable_IsFalse()
        {
            IHttpHandler mrCMSHttpHandler = new MrCMSHttpHandler(A.Fake<RequestContext>(), null, null);

            mrCMSHttpHandler.IsReusable.Should().BeFalse();
        }

        [Fact]
        public void MrCMSHttpHandler_IsAllowed_ReturnsTrueIfCurrentUserIsAllowedForWebpage()
        {
            var mrCMSHttpHandler = new MrCMSHttpHandler(A.Fake<RequestContext>(), null, null);
            var httpContext = A.Fake<HttpContextBase>();
            mrCMSHttpHandler.Webpage = new StubAllowedWebpage();

            mrCMSHttpHandler.IsAllowed(httpContext).Should().BeTrue();
        }

        [Fact]
        public void MrCMSHttpHandler_IsAllowed_ReturnsFalseIfCurrentUserIsDisallowedForWebpage()
        {
            var mrCMSHttpHandler = new MrCMSHttpHandler(A.Fake<RequestContext>(), null, null);
            var httpContext = A.Fake<HttpContextBase>();
            mrCMSHttpHandler.Webpage = new StubDisallowedWebpage();

            mrCMSHttpHandler.IsAllowed(httpContext).Should().BeFalse();
        }

        [Fact]
        public void MrCMSHttpHandler_IsAllowed_ShouldRedirectToRootIfDisallowed()
        {
            var mrCMSHttpHandler = new MrCMSHttpHandler(A.Fake<RequestContext>(), null, null);
            var httpContext = A.Fake<HttpContextBase>();
            mrCMSHttpHandler.Webpage = new StubDisallowedWebpage();

            mrCMSHttpHandler.IsAllowed(httpContext);

            A.CallTo(() => httpContext.Response.Redirect("~")).MustHaveHappened();
        }

        [Fact]
        public void MrCMSHttpHandler_Handle500_CallsSiteSettings500PageId()
        {
            var siteSettings = A.Fake<SiteSettings>();
            var documentService = A.Fake<IDocumentService>();
            var mrCMSHttpHandler = new MrCMSHttpHandler(A.Fake<RequestContext>(), () => documentService, () => siteSettings);
            var httpContext = A.Fake<HttpContextBase>();

            mrCMSHttpHandler.Handle500(httpContext, new Exception());

            A.CallTo(() => siteSettings.Error500PageId).MustHaveHappened();
        }

        [Fact]
        public void MrCMSHttpHandler_Handle500_CallsGetDocumentWithResultOf500Page()
        {
            var siteSettings = A.Fake<SiteSettings>();
            A.CallTo(() => siteSettings.Error500PageId).Returns(1);
            var documentService = A.Fake<IDocumentService>();
            var mrCMSHttpHandler = new MrCMSHttpHandler(A.Fake<RequestContext>(), () => documentService, () => siteSettings);
            var httpContext = A.Fake<HttpContextBase>();

            mrCMSHttpHandler.Handle500(httpContext, new Exception());

            A.CallTo(() => documentService.GetDocument<Webpage>(1)).MustHaveHappened();
        }

        [Fact]
        public void MrCMSHttpHandler_Handle500_500DocumentFoundRedirectsToThatUrl()
        {
            var siteSettings = A.Fake<SiteSettings>();
            A.CallTo(() => siteSettings.Error500PageId).Returns(1);
            var documentService = A.Fake<IDocumentService>();
            A.CallTo(() => documentService.GetDocument<Webpage>(1)).Returns(new TextPage { UrlSegment = "test-500" });
            var mrCMSHttpHandler = new MrCMSHttpHandler(A.Fake<RequestContext>(), () => documentService, () => siteSettings);
            var httpContext = A.Fake<HttpContextBase>();

            mrCMSHttpHandler.Handle500(httpContext, new Exception());

            A.CallTo(() => httpContext.Response.Redirect("~/test-500")).MustHaveHappened();
        }

        [Fact]
        public void MrCMSHttpHandler_Handle500_500DocumentFoundRedirectsToRoot()
        {
            var siteSettings = A.Fake<SiteSettings>();
            A.CallTo(() => siteSettings.Error500PageId).Returns(1);
            var documentService = A.Fake<IDocumentService>();
            A.CallTo(() => documentService.GetDocument<Webpage>(1)).Returns(null);
            var mrCMSHttpHandler = new MrCMSHttpHandler(A.Fake<RequestContext>(), () => documentService, () => siteSettings);
            var httpContext = A.Fake<HttpContextBase>();

            mrCMSHttpHandler.Handle500(httpContext, new Exception());

            A.CallTo(() => httpContext.Response.Redirect("~")).MustHaveHappened();
        }

        [Fact]
        public void MrCMSHttpHandler_GetControllerName_NullWebpageReturnsNull()
        {
            var mrCMSHttpHandler = new MrCMSHttpHandler(A.Fake<RequestContext>(), null, null)
                                       {
                                           Webpage = null
                                       };

            mrCMSHttpHandler.GetControllerName().Should().BeNull();
        }

        [Fact]
        public void MrCMSHttpHandler_GetControllerName_WebpageNotPublishedAndNotAllowedReturnsNull()
        {
            var mrCMSHttpHandler = new MrCMSHttpHandler(A.Fake<RequestContext>(), null, null)
            {
                Webpage = new StubDisallowedWebpage()
            };

            mrCMSHttpHandler.GetControllerName().Should().BeNull();
        }

        [Fact]
        public void MrCMSHttpHandler_GetControllerName_NullDocumentTypeDefinitionReturnsNull()
        {
            var documentService = A.Fake<IDocumentService>();
            A.CallTo(() => documentService.GetDefinitionByType(typeof(StubAllowedWebpage))).Returns(null);
            var mrCMSHttpHandler = new MrCMSHttpHandler(A.Fake<RequestContext>(), () => documentService, null)
            {
                Webpage = new StubAllowedWebpage { PublishOn = DateTime.UtcNow.AddDays(-1) }
            };

            mrCMSHttpHandler.GetControllerName().Should().BeNull();
        }

        [Fact]
        public void MrCMSHttpHandler_GetControllerName_HttpMethodIsGETReturnsWebGetController()
        {
            var documentService = A.Fake<IDocumentService>();
            var documentTypeDefinition = new DocumentTypeDefinition(ChildrenListType.WhiteList)
            {
                WebGetController = "test-controller"
            };
            A.CallTo(() => documentService.GetDefinitionByType(typeof(StubAllowedWebpage))).Returns(documentTypeDefinition);
            var mrCMSHttpHandler = new MrCMSHttpHandler(A.Fake<RequestContext>(), () => documentService, null)
            {
                Webpage = new StubAllowedWebpage { PublishOn = DateTime.UtcNow.AddDays(-1) },
                HttpMethod = "GET"
            };

            mrCMSHttpHandler.GetControllerName().Should().Be("test-controller");
        }

        [Fact]
        public void MrCMSHttpHandler_GetControllerName_HttpMethodIsPOSTReturnsWebGetController()
        {
            var documentService = A.Fake<IDocumentService>();
            var documentTypeDefinition = new DocumentTypeDefinition(ChildrenListType.WhiteList)
            {
                WebPostController = "test-controller"
            };
            A.CallTo(() => documentService.GetDefinitionByType(typeof(StubAllowedWebpage))).Returns(documentTypeDefinition);
            var mrCMSHttpHandler = new MrCMSHttpHandler(A.Fake<RequestContext>(), () => documentService, null)
            {
                Webpage = new StubAllowedWebpage { PublishOn = DateTime.UtcNow.AddDays(-1) },
                HttpMethod = "POST"
            };

            mrCMSHttpHandler.GetControllerName().Should().Be("test-controller");
        }

        [Fact]
        public void MrCMSHttpHandler_GetControllerName_HttpMethodIsAnotherTypeReturnsNull()
        {
            var documentService = A.Fake<IDocumentService>();
            var documentTypeDefinition = new DocumentTypeDefinition(ChildrenListType.WhiteList)
            {
                WebPostController = "test-controller"
            };
            A.CallTo(() => documentService.GetDefinitionByType(typeof(StubAllowedWebpage))).Returns(documentTypeDefinition);
            var mrCMSHttpHandler = new MrCMSHttpHandler(A.Fake<RequestContext>(), () => documentService, null)
            {
                Webpage = new StubAllowedWebpage { PublishOn = DateTime.UtcNow.AddDays(-1) },
                HttpMethod = "PUT"
            };

            mrCMSHttpHandler.GetControllerName().Should().BeNull();
        }

        [Fact]
        public void MrCMSHttpHandler_Handle404_MustCallSiteSettings404PageId()
        {
            var siteSettings = A.Fake<SiteSettings>();
            var documentService = A.Fake<IDocumentService>();
            var mrCMSHttpHandler = new MrCMSHttpHandler(A.Fake<RequestContext>(), () => documentService, () => siteSettings);
            var httpContext = A.Fake<HttpContextBase>();

            mrCMSHttpHandler.Handle404(httpContext);

            A.CallTo(() => siteSettings.Error404PageId).MustHaveHappened();
        }

        [Fact]
        public void MrCMSHttpHandler_Handle404_MustCallDocumentServiceWithTheResultOfThe404()
        {
            var siteSettings = A.Fake<SiteSettings>();
            A.CallTo(() => siteSettings.Error404PageId).Returns(1);
            var documentService = A.Fake<IDocumentService>();
            var mrCMSHttpHandler = new MrCMSHttpHandler(A.Fake<RequestContext>(), () => documentService, () => siteSettings);
            var httpContext = A.Fake<HttpContextBase>();

            mrCMSHttpHandler.Handle404(httpContext);

            A.CallTo(() => documentService.GetDocument<Webpage>(1)).MustHaveHappened();
        }

        [Fact]
        public void MrCMSHttpHandler_Handle404_404DocumentFoundRedirectsToThatUrl()
        {
            var siteSettings = A.Fake<SiteSettings>();
            A.CallTo(() => siteSettings.Error404PageId).Returns(1);
            var documentService = A.Fake<IDocumentService>();
            A.CallTo(() => documentService.GetDocument<Webpage>(1)).Returns(new TextPage { UrlSegment = "test-404" });
            var mrCMSHttpHandler = new MrCMSHttpHandler(A.Fake<RequestContext>(), () => documentService, () => siteSettings);
            var httpContext = A.Fake<HttpContextBase>();

            mrCMSHttpHandler.Handle404(httpContext);

            A.CallTo(() => httpContext.Response.Redirect("~/test-404")).MustHaveHappened();
        }

        [Fact]
        public void MrCMSHttpHandler_Handle404_WebpageSetReturnsFalse()
        {
            var mrCMSHttpHandler = new MrCMSHttpHandler(A.Fake<RequestContext>(), null, null)
                                       {
                                           Webpage = new TextPage()
                                       };
            var httpContext = A.Fake<HttpContextBase>();

            mrCMSHttpHandler.Handle404(httpContext).Should().BeFalse();
        }

        [Fact]
        public void MrCMSHttpHandler_Handle404_404DocumentNotFoundRedirectsToRoot()
        {
            var siteSettings = A.Fake<SiteSettings>();
            A.CallTo(() => siteSettings.Error404PageId).Returns(1);
            var documentService = A.Fake<IDocumentService>();
            A.CallTo(() => documentService.GetDocument<Webpage>(1)).Returns(null);
            var mrCMSHttpHandler = new MrCMSHttpHandler(A.Fake<RequestContext>(), () => documentService, () => siteSettings);
            var httpContext = A.Fake<HttpContextBase>();

            mrCMSHttpHandler.Handle404(httpContext);

            A.CallTo(() => httpContext.Response.Redirect("~")).MustHaveHappened();
        }

        [Fact]
        public void MrCMSHttpHandler_RedirectsToHomePage_ReturnFalseIfUrlSegmentSameAsLiveUrlSegment()
        {
            var mrCMSHttpHandler = new MrCMSHttpHandler(A.Fake<RequestContext>(), null, null)
            {
                Webpage = new TextPage { UrlSegment = "test" }
            };
            var httpContext = A.Fake<HttpContextBase>();

            mrCMSHttpHandler.RedirectsToHomePage(httpContext).Should().BeFalse();
        }

        [Fact]
        public void MrCMSHttpHandler_RedirectsToHomePage_ReturnTrueIfItIsHomePage()
        {
            var textPage = new TextPage { UrlSegment = "test", Site = new Site(), PublishOn = DateTime.Today.AddDays(-1) };
            MrCMSApplication.OverridenRootChildren = new List<Webpage> { textPage };
            var mrCMSHttpHandler = new MrCMSHttpHandler(A.Fake<RequestContext>(), null, null)
            {
                Webpage = textPage
            };
            var httpContext = A.Fake<HttpContextBase>();

            A.CallTo(() => httpContext.Request.Url).Returns(new Uri("http://www.example.com/test"));

            mrCMSHttpHandler.RedirectsToHomePage(httpContext).Should().BeTrue();
        }

        [Fact]
        public void MrCMSHttpHandler_RedirectsToHomePage_RedirectsToRootIfItIsHomePage()
        {
            var textPage = new TextPage { UrlSegment = "test", Site = new Site(), PublishOn = DateTime.Today.AddDays(-1) };
            MrCMSApplication.OverridenRootChildren = new List<Webpage> { textPage };
            var mrCMSHttpHandler = new MrCMSHttpHandler(A.Fake<RequestContext>(), null, null)
            {
                Webpage = textPage
            };
            var httpContext = A.Fake<HttpContextBase>();

            A.CallTo(() => httpContext.Request.Url).Returns(new Uri("http://www.example.com/test"));

            mrCMSHttpHandler.RedirectsToHomePage(httpContext);

            A.CallTo(() => httpContext.Response.Redirect("~/")).MustHaveHappened();
        }

        [Fact]
        public void MrCMSHttpHandler_RedirectsToHomePage_FalseIfAlreadyRedirected()
        {
            var textPage = new TextPage { UrlSegment = "test", Site = new Site(), PublishOn = DateTime.Today.AddDays(-1) };
            MrCMSApplication.OverridenRootChildren = new List<Webpage> { textPage };
            var mrCMSHttpHandler = new MrCMSHttpHandler(A.Fake<RequestContext>(), null, null)
            {
                Webpage = textPage
            };
            var httpContext = A.Fake<HttpContextBase>();

            A.CallTo(() => httpContext.Request.Url).Returns(new Uri("http://www.example.com/"));

            mrCMSHttpHandler.RedirectsToHomePage(httpContext).Should().BeFalse();
        }

        [Fact]
        public void MrCMSHttpHandler_PageIsRedirect_NullWebpageReturnsFalse()
        {
            var mrCMSHttpHandler = new MrCMSHttpHandler(A.Fake<RequestContext>(), null, null)
            {
                Webpage = null
            };
            var httpContext = A.Fake<HttpContextBase>();

            mrCMSHttpHandler.PageIsRedirect(httpContext).Should().BeFalse();
        }

        [Fact]
        public void MrCMSHttpHandler_PageIsRedirect_NonRedirectSetReturnsFalse()
        {
            var mrCMSHttpHandler = new MrCMSHttpHandler(A.Fake<RequestContext>(), null, null)
            {
                Webpage = new TextPage()
            };
            var httpContext = A.Fake<HttpContextBase>();

            mrCMSHttpHandler.PageIsRedirect(httpContext).Should().BeFalse();
        }

        [Fact]
        public void MrCMSHttpHandler_PageIsRedirect_RedirectSetReturnsTrue()
        {
            var mrCMSHttpHandler = new MrCMSHttpHandler(A.Fake<RequestContext>(), null, null)
            {
                Webpage = new Redirect { RedirectUrl = "test-redirect" }
            };
            var httpContext = A.Fake<HttpContextBase>();

            mrCMSHttpHandler.PageIsRedirect(httpContext).Should().BeTrue();
        }

        [Fact]
        public void MrCMSHttpHandler_PageIsRedirect_RedirectSetShouldCallResponseRedirectForTheRedirectUrl()
        {
            var mrCMSHttpHandler = new MrCMSHttpHandler(A.Fake<RequestContext>(), null, null)
            {
                Webpage = new Redirect { RedirectUrl = "test-redirect" }
            };
            var httpContext = A.Fake<HttpContextBase>();

            mrCMSHttpHandler.PageIsRedirect(httpContext);
            A.CallTo(() => httpContext.Response.Redirect("~/test-redirect")).MustHaveHappened();
        }

        [Fact]
        public void MrCMSHttpHandler_PageIsRedirect_IfRedirectIsAbsoluteUrlShouldRedirectWithoutTilde()
        {
            var mrCMSHttpHandler = new MrCMSHttpHandler(A.Fake<RequestContext>(), null, null)
            {
                Webpage = new Redirect { RedirectUrl = "http://www.example.com" }
            };
            var httpContext = A.Fake<HttpContextBase>();

            mrCMSHttpHandler.PageIsRedirect(httpContext);
            A.CallTo(() => httpContext.Response.Redirect("http://www.example.com")).MustHaveHappened();
        }

        [Fact]
        public void MrCMSHttpHandler_PageIsRedirect_RedirectSetRedirectUrlStartsWithBackSlashItShouldNotAddExtraSlash()
        {
            var mrCMSHttpHandler = new MrCMSHttpHandler(A.Fake<RequestContext>(), null, null)
            {
                Webpage = new Redirect { RedirectUrl = "/test-redirect" }
            };
            var httpContext = A.Fake<HttpContextBase>();

            mrCMSHttpHandler.PageIsRedirect(httpContext);
            A.CallTo(() => httpContext.Response.Redirect("~/test-redirect")).MustHaveHappened();
        }

        [Fact]
        public void MrCMSHttpHandler_GetActionName_WebpageIsNullReturnNull()
        {
            var mrCMSHttpHandler = new MrCMSHttpHandler(A.Fake<RequestContext>(), null, null)
            {
                Webpage = null
            };

            mrCMSHttpHandler.GetActionName().Should().BeNull();
        }

        [Fact]
        public void MrCMSHttpHandler_GetActionName_WebpageNotAllowedAndUnpublishedReturnsNull()
        {
            var mrCMSHttpHandler = new MrCMSHttpHandler(A.Fake<RequestContext>(), null, null)
            {
                Webpage = new StubDisallowedWebpage()
            };

            mrCMSHttpHandler.GetActionName().Should().BeNull();
        }

        [Fact]
        public void MrCMSHttpHandler_GetActionName_NullDocumentTypeDefinitionReturnsNull()
        {
            var documentService = A.Fake<IDocumentService>();
            A.CallTo(() => documentService.GetDefinitionByType(typeof(StubAllowedWebpage))).Returns(null);
            var mrCMSHttpHandler = new MrCMSHttpHandler(A.Fake<RequestContext>(), () => documentService, null)
            {
                Webpage = new StubAllowedWebpage { PublishOn = DateTime.UtcNow.AddDays(-1) }
            };

            mrCMSHttpHandler.GetActionName().Should().BeNull();
            A.CallTo(() => documentService.GetDefinitionByType(typeof(StubAllowedWebpage))).MustHaveHappened();
        }

        [Fact]
        public void MrCMSHttpHandler_GetActionName_ReturnsDefinitionWebGetActionIfHttpMethodIsGET()
        {
            var documentService = A.Fake<IDocumentService>();
            var documentTypeDefinition = new DocumentTypeDefinition(ChildrenListType.WhiteList)
            {
                WebGetAction = "test-get-action"
            };
            A.CallTo(() => documentService.GetDefinitionByType(typeof(StubAllowedWebpage))).Returns(documentTypeDefinition);
            var mrCMSHttpHandler = new MrCMSHttpHandler(A.Fake<RequestContext>(), () => documentService, null)
            {
                Webpage = new StubAllowedWebpage { PublishOn = DateTime.UtcNow.AddDays(-1) },
                HttpMethod = "GET"
            };

            mrCMSHttpHandler.GetActionName().Should().Be("test-get-action");
        }

        [Fact]
        public void MrCMSHttpHandler_GetActionName_ReturnsDefinitionWebGetActionIfHttpMethodIsPOST()
        {
            var documentService = A.Fake<IDocumentService>();
            var documentTypeDefinition = new DocumentTypeDefinition(ChildrenListType.WhiteList)
            {
                WebPostAction = "test-post-action"
            };
            A.CallTo(() => documentService.GetDefinitionByType(typeof(StubAllowedWebpage))).Returns(documentTypeDefinition);
            var mrCMSHttpHandler = new MrCMSHttpHandler(A.Fake<RequestContext>(), () => documentService, null)
            {
                Webpage = new StubAllowedWebpage { PublishOn = DateTime.UtcNow.AddDays(-1) },
                HttpMethod = "POST"
            };

            mrCMSHttpHandler.GetActionName().Should().Be("test-post-action");
        }

        [Fact]
        public void MrCMSHttpHandler_GetActionName_ReturnsNullIfHttpMethodIsSomethingElse()
        {
            var documentService = A.Fake<IDocumentService>();
            var documentTypeDefinition = new DocumentTypeDefinition(ChildrenListType.WhiteList)
            {
                WebPostAction = "test-post-action"
            };
            A.CallTo(() => documentService.GetDefinitionByType(typeof(StubAllowedWebpage))).Returns(documentTypeDefinition);
            var mrCMSHttpHandler = new MrCMSHttpHandler(A.Fake<RequestContext>(), () => documentService, null)
            {
                Webpage = new StubAllowedWebpage { PublishOn = DateTime.UtcNow.AddDays(-1) },
                HttpMethod = "PUT"
            };

            mrCMSHttpHandler.GetActionName().Should().BeNull();
        }

        [Fact]
        public void MrCMSHttpHandler_CheckIsFile_UrlEndsWithoutExtensionReturnsFalse()
        {
            var requestContext = A.Fake<RequestContext>();
            A.CallTo(() => requestContext.HttpContext.Request.Url.ToString()).Returns("test-page");
            var mrCMSHttpHandler = new MrCMSHttpHandler(requestContext, null, null);

            mrCMSHttpHandler.CheckIsFile().Should().BeFalse();
        }

        [Fact]
        public void MrCMSHttpHandler_CheckIsFile_UrlEndsWithExtensionReturnsTrue()
        {
            var requestContext = A.Fake<RequestContext>();
            A.CallTo(() => requestContext.HttpContext.Request.Url.ToString()).Returns("test-page.jpg");
            var mrCMSHttpHandler = new MrCMSHttpHandler(requestContext, null, null);

            mrCMSHttpHandler.CheckIsFile().Should().BeTrue();
        }

        [Fact]
        public void MrCMSHttpHandler_SetFormData_IfHttpMethodIsPOSTAndTheFormDataIsNotNullSetTheRouteData()
        {
            var requestContext = A.Fake<RequestContext>();
            A.CallTo(() => requestContext.HttpContext.Request.Form)
             .Returns(new NameValueCollection { { "test", "data" } });
            var mrCMSHttpHandler = new MrCMSHttpHandler(requestContext, null, null) { HttpMethod = "POST" };

            var controller = A.Fake<Controller>();
            var routeData = new RouteData();
            controller.ControllerContext = new ControllerContext { RouteData = routeData };
            mrCMSHttpHandler.SetFormData(controller);

            routeData.Values["form"].Should().NotBeNull();
        }
    }
}