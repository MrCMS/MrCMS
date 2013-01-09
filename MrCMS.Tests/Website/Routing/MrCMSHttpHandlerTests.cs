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
using NHibernate;
using Xunit;

namespace MrCMS.Tests.Website.Routing
{
    public class MrCMSHttpHandlerTests
    {
        private SiteSettings siteSettings;
        private IDocumentService documentService;
        private ISession session;
        private RequestContext requestContext;

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
            var mrCMSHttpHandler = GetMrCMSHttpHandler();
            var httpContext = A.Fake<HttpContextBase>();

            mrCMSHttpHandler.CheckIsInstalled(httpContext).Should().BeFalse();

            A.CallTo(() => httpContext.Response.Redirect("~/Install")).MustHaveHappened();
        }

        [Fact]
        public void MrCMSHttpHandler_CheckIsInstalled_DatabaseIsInstalledReturnsTrue()
        {
            var mrCMSHttpHandler = GetMrCMSHttpHandler();
            var httpContext = A.Fake<HttpContextBase>();

            mrCMSHttpHandler.CheckIsInstalled(httpContext).Should().BeTrue();
        }

        [Fact]
        public void MrCMSHttpHandler_IsReusable_IsFalse()
        {
            IHttpHandler mrCMSHttpHandler = GetMrCMSHttpHandler();

            mrCMSHttpHandler.IsReusable.Should().BeFalse();
        }

        [Fact]
        public void MrCMSHttpHandler_IsAllowed_ReturnsTrueIfCurrentUserIsAllowedForWebpage()
        {
            var mrCMSHttpHandler = GetMrCMSHttpHandler();
            var httpContext = A.Fake<HttpContextBase>();
            mrCMSHttpHandler.Webpage = new StubAllowedWebpage();

            mrCMSHttpHandler.IsAllowed(httpContext).Should().BeTrue();
        }

        [Fact]
        public void MrCMSHttpHandler_IsAllowed_ReturnsFalseIfCurrentUserIsDisallowedForWebpage()
        {
            var mrCMSHttpHandler = GetMrCMSHttpHandler();
            var httpContext = A.Fake<HttpContextBase>();
            mrCMSHttpHandler.Webpage = new StubDisallowedWebpage();

            mrCMSHttpHandler.IsAllowed(httpContext).Should().BeFalse();
        }

        [Fact]
        public void MrCMSHttpHandler_IsAllowed_ShouldRedirectToRootIfDisallowed()
        {
            var mrCMSHttpHandler = GetMrCMSHttpHandler();
            var httpContext = A.Fake<HttpContextBase>();
            mrCMSHttpHandler.Webpage = new StubDisallowedWebpage();

            mrCMSHttpHandler.IsAllowed(httpContext);

            A.CallTo(() => httpContext.Response.Redirect("~")).MustHaveHappened();
        }

        [Fact]
        public void MrCMSHttpHandler_Handle500_CallsSiteSettings500PageId()
        {
            var mrCMSHttpHandler = GetMrCMSHttpHandler();
            var httpContext = A.Fake<HttpContextBase>();

            mrCMSHttpHandler.Handle500(httpContext, new Exception());

            A.CallTo(() => siteSettings.Error500PageId).MustHaveHappened();
        }

        [Fact]
        public void MrCMSHttpHandler_Handle500_CallsGetDocumentWithResultOf500Page()
        {
            var mrCMSHttpHandler = GetMrCMSHttpHandler();
            A.CallTo(() => siteSettings.Error500PageId).Returns(1);
            var httpContext = A.Fake<HttpContextBase>();

            mrCMSHttpHandler.Handle500(httpContext, new Exception());

            A.CallTo(() => documentService.GetDocument<Webpage>(1)).MustHaveHappened();
        }

        [Fact]
        public void MrCMSHttpHandler_Handle500_500DocumentFoundRedirectsToThatUrl()
        {
            var mrCMSHttpHandler = GetMrCMSHttpHandler();
            A.CallTo(() => siteSettings.Error500PageId).Returns(1);
            A.CallTo(() => documentService.GetDocument<Webpage>(1)).Returns(new BasicMappedWebpage { UrlSegment = "test-500" });
            var httpContext = A.Fake<HttpContextBase>();

            mrCMSHttpHandler.Handle500(httpContext, new Exception());

            A.CallTo(() => httpContext.Response.Redirect("~/test-500")).MustHaveHappened();
        }

        [Fact]
        public void MrCMSHttpHandler_Handle500_500DocumentFoundRedirectsToRoot()
        {
            var mrCMSHttpHandler = GetMrCMSHttpHandler();
            A.CallTo(() => siteSettings.Error500PageId).Returns(1);
            A.CallTo(() => documentService.GetDocument<Webpage>(1)).Returns(null);
            var httpContext = A.Fake<HttpContextBase>();

            mrCMSHttpHandler.Handle500(httpContext, new Exception());

            A.CallTo(() => httpContext.Response.Redirect("~")).MustHaveHappened();
        }

        [Fact]
        public void MrCMSHttpHandler_GetControllerName_NullWebpageReturnsNull()
        {
            var mrCMSHttpHandler = GetMrCMSHttpHandler();
            mrCMSHttpHandler.Webpage = null;

            mrCMSHttpHandler.GetControllerName().Should().BeNull();
        }

        [Fact]
        public void MrCMSHttpHandler_GetControllerName_WebpageNotPublishedAndNotAllowedReturnsNull()
        {
            var mrCMSHttpHandler = GetMrCMSHttpHandler();
            mrCMSHttpHandler.Webpage = new StubDisallowedWebpage();

            mrCMSHttpHandler.GetControllerName().Should().BeNull();
        }

        [Fact]
        public void MrCMSHttpHandler_GetControllerName_NullDocumentTypeDefinitionReturnsNull()
        {
            var mrCMSHttpHandler = GetMrCMSHttpHandler();
            A.CallTo(() => documentService.GetDefinitionByType(typeof(StubAllowedWebpage))).Returns(null);
            mrCMSHttpHandler.Webpage = new StubAllowedWebpage { PublishOn = DateTime.UtcNow.AddDays(-1) };

            mrCMSHttpHandler.GetControllerName().Should().BeNull();
        }

        [Fact]
        public void MrCMSHttpHandler_GetControllerName_HttpMethodIsGETReturnsWebGetController()
        {
            var mrCMSHttpHandler = GetMrCMSHttpHandler();
            var documentTypeDefinition = new DocumentTypeDefinition(ChildrenListType.WhiteList)
            {
                WebGetController = "test-controller"
            };
            A.CallTo(() => documentService.GetDefinitionByType(typeof(StubAllowedWebpage))).Returns(documentTypeDefinition);
            mrCMSHttpHandler.Webpage = new StubAllowedWebpage { PublishOn = DateTime.UtcNow.AddDays(-1) };
            mrCMSHttpHandler.HttpMethod = "GET";

            mrCMSHttpHandler.GetControllerName().Should().Be("test-controller");
        }

        [Fact]
        public void MrCMSHttpHandler_GetControllerName_HttpMethodIsPOSTReturnsWebGetController()
        {
            var mrCMSHttpHandler = GetMrCMSHttpHandler();
            var documentTypeDefinition = new DocumentTypeDefinition(ChildrenListType.WhiteList)
            {
                WebPostController = "test-controller"
            };
            A.CallTo(() => documentService.GetDefinitionByType(typeof(StubAllowedWebpage))).Returns(documentTypeDefinition);
            mrCMSHttpHandler.Webpage = new StubAllowedWebpage { PublishOn = DateTime.UtcNow.AddDays(-1) };
            mrCMSHttpHandler.HttpMethod = "POST";

            mrCMSHttpHandler.GetControllerName().Should().Be("test-controller");
        }

        [Fact]
        public void MrCMSHttpHandler_GetControllerName_HttpMethodIsAnotherTypeReturnsNull()
        {
            var mrCMSHttpHandler = GetMrCMSHttpHandler();
            var documentTypeDefinition = new DocumentTypeDefinition(ChildrenListType.WhiteList)
            {
                WebPostController = "test-controller"
            };
            A.CallTo(() => documentService.GetDefinitionByType(typeof(StubAllowedWebpage))).Returns(documentTypeDefinition);
            mrCMSHttpHandler.Webpage = new StubAllowedWebpage { PublishOn = DateTime.UtcNow.AddDays(-1) };
            mrCMSHttpHandler.HttpMethod = "PUT";

            mrCMSHttpHandler.GetControllerName().Should().BeNull();
        }

        [Fact]
        public void MrCMSHttpHandler_Handle404_MustCallSiteSettings404PageId()
        {
            var mrCMSHttpHandler = GetMrCMSHttpHandler();
            var httpContext = A.Fake<HttpContextBase>();

            mrCMSHttpHandler.Handle404(httpContext);

            A.CallTo(() => siteSettings.Error404PageId).MustHaveHappened();
        }

        [Fact]
        public void MrCMSHttpHandler_Handle404_MustCallDocumentServiceWithTheResultOfThe404()
        {
            var mrCMSHttpHandler = GetMrCMSHttpHandler();
            A.CallTo(() => siteSettings.Error404PageId).Returns(1);
            var httpContext = A.Fake<HttpContextBase>();

            mrCMSHttpHandler.Handle404(httpContext);

            A.CallTo(() => documentService.GetDocument<Webpage>(1)).MustHaveHappened();
        }

        [Fact]
        public void MrCMSHttpHandler_Handle404_404DocumentFoundRedirectsToThatUrl()
        {
            var mrCMSHttpHandler = GetMrCMSHttpHandler();
            A.CallTo(() => siteSettings.Error404PageId).Returns(1);
            A.CallTo(() => documentService.GetDocument<Webpage>(1)).Returns(new BasicMappedWebpage { UrlSegment = "test-404" });
            var httpContext = A.Fake<HttpContextBase>();

            mrCMSHttpHandler.Handle404(httpContext);

            A.CallTo(() => httpContext.Response.Redirect("~/test-404")).MustHaveHappened();
        }

        [Fact]
        public void MrCMSHttpHandler_Handle404_WebpageSetReturnsFalse()
        {
            var mrCMSHttpHandler = GetMrCMSHttpHandler();
            mrCMSHttpHandler.Webpage = new BasicMappedWebpage();
            var httpContext = A.Fake<HttpContextBase>();

            mrCMSHttpHandler.Handle404(httpContext).Should().BeFalse();
        }

        [Fact]
        public void MrCMSHttpHandler_Handle404_404DocumentNotFoundRedirectsToRoot()
        {
            var mrCMSHttpHandler = GetMrCMSHttpHandler();
            A.CallTo(() => siteSettings.Error404PageId).Returns(1);
            A.CallTo(() => documentService.GetDocument<Webpage>(1)).Returns(null);
            var httpContext = A.Fake<HttpContextBase>();

            mrCMSHttpHandler.Handle404(httpContext);

            A.CallTo(() => httpContext.Response.Redirect("~")).MustHaveHappened();
        }

        [Fact]
        public void MrCMSHttpHandler_RedirectsToHomePage_ReturnFalseIfUrlSegmentSameAsLiveUrlSegment()
        {
            var mrCMSHttpHandler = GetMrCMSHttpHandler();
            mrCMSHttpHandler.Webpage = new BasicMappedWebpage { UrlSegment = "test" };
            var httpContext = A.Fake<HttpContextBase>();

            mrCMSHttpHandler.RedirectsToHomePage(httpContext).Should().BeFalse();
        }

        [Fact]
        public void MrCMSHttpHandler_RedirectsToHomePage_ReturnTrueIfItIsHomePage()
        {
            var mrCMSHttpHandler = GetMrCMSHttpHandler();
            var textPage = new BasicMappedWebpage { UrlSegment = "test", Site = new Site(), PublishOn = DateTime.Today.AddDays(-1) };
            MrCMSApplication.OverridenRootChildren = new List<Webpage> { textPage };
            mrCMSHttpHandler.Webpage = textPage;
            var httpContext = A.Fake<HttpContextBase>();

            A.CallTo(() => httpContext.Request.Url).Returns(new Uri("http://www.example.com/test"));

            mrCMSHttpHandler.RedirectsToHomePage(httpContext).Should().BeTrue();
        }

        [Fact]
        public void MrCMSHttpHandler_RedirectsToHomePage_RedirectsToRootIfItIsHomePage()
        {
            var mrCMSHttpHandler = GetMrCMSHttpHandler();
            var textPage = new BasicMappedWebpage { UrlSegment = "test", Site = new Site(), PublishOn = DateTime.Today.AddDays(-1) };
            MrCMSApplication.OverridenRootChildren = new List<Webpage> { textPage };
            mrCMSHttpHandler.Webpage = textPage;
            var httpContext = A.Fake<HttpContextBase>();

            A.CallTo(() => httpContext.Request.Url).Returns(new Uri("http://www.example.com/test"));

            mrCMSHttpHandler.RedirectsToHomePage(httpContext);

            A.CallTo(() => httpContext.Response.Redirect("~/")).MustHaveHappened();
        }

        [Fact]
        public void MrCMSHttpHandler_RedirectsToHomePage_FalseIfAlreadyRedirected()
        {
            var mrCMSHttpHandler = GetMrCMSHttpHandler();
            var textPage = new BasicMappedWebpage { UrlSegment = "test", Site = new Site(), PublishOn = DateTime.Today.AddDays(-1) };
            MrCMSApplication.OverridenRootChildren = new List<Webpage> { textPage };
            mrCMSHttpHandler.Webpage = textPage;
            var httpContext = A.Fake<HttpContextBase>();

            A.CallTo(() => httpContext.Request.Url).Returns(new Uri("http://www.example.com/"));

            mrCMSHttpHandler.RedirectsToHomePage(httpContext).Should().BeFalse();
        }

        [Fact]
        public void MrCMSHttpHandler_PageIsRedirect_NullWebpageReturnsFalse()
        {
            var mrCMSHttpHandler = GetMrCMSHttpHandler();
            mrCMSHttpHandler.Webpage = null;
            var httpContext = A.Fake<HttpContextBase>();

            mrCMSHttpHandler.PageIsRedirect(httpContext).Should().BeFalse();
        }

        [Fact]
        public void MrCMSHttpHandler_PageIsRedirect_NonRedirectSetReturnsFalse()
        {
            var mrCMSHttpHandler = GetMrCMSHttpHandler();
            mrCMSHttpHandler.Webpage = new BasicMappedWebpage();
            var httpContext = A.Fake<HttpContextBase>();

            mrCMSHttpHandler.PageIsRedirect(httpContext).Should().BeFalse();
        }

        [Fact]
        public void MrCMSHttpHandler_PageIsRedirect_RedirectSetReturnsTrue()
        {
            var mrCMSHttpHandler = GetMrCMSHttpHandler();
            mrCMSHttpHandler.Webpage = new Redirect { RedirectUrl = "test-redirect" };
            var httpContext = A.Fake<HttpContextBase>();

            mrCMSHttpHandler.PageIsRedirect(httpContext).Should().BeTrue();
        }

        [Fact]
        public void MrCMSHttpHandler_PageIsRedirect_RedirectSetShouldCallResponseRedirectForTheRedirectUrl()
        {
            var mrCMSHttpHandler = GetMrCMSHttpHandler();
            mrCMSHttpHandler.Webpage = new Redirect { RedirectUrl = "test-redirect" };
            var httpContext = A.Fake<HttpContextBase>();

            mrCMSHttpHandler.PageIsRedirect(httpContext);
            A.CallTo(() => httpContext.Response.Redirect("~/test-redirect")).MustHaveHappened();
        }

        [Fact]
        public void MrCMSHttpHandler_PageIsRedirect_RedirectWithPermanentCallsResponseRedirectPermanentForTheRedirectUrl()
        {
            var mrCMSHttpHandler = GetMrCMSHttpHandler();
            mrCMSHttpHandler.Webpage = new Redirect {RedirectUrl = "test-redirect", Permanent = true};
            var httpContext = A.Fake<HttpContextBase>();

            mrCMSHttpHandler.PageIsRedirect(httpContext);
            A.CallTo(() => httpContext.Response.RedirectPermanent("~/test-redirect")).MustHaveHappened();
        }

        [Fact]
        public void MrCMSHttpHandler_PageIsRedirect_IfRedirectIsAbsoluteUrlShouldRedirectWithoutTilde()
        {
            var mrCMSHttpHandler = GetMrCMSHttpHandler();
            mrCMSHttpHandler.Webpage = new Redirect { RedirectUrl = "http://www.example.com" };
            var httpContext = A.Fake<HttpContextBase>();

            mrCMSHttpHandler.PageIsRedirect(httpContext);
            A.CallTo(() => httpContext.Response.Redirect("http://www.example.com")).MustHaveHappened();
        }

        [Fact]
        public void MrCMSHttpHandler_PageIsRedirect_IfRedirectIsAbsoluteUrlAndIsPermanentShouldBe301()
        {
            var mrCMSHttpHandler = GetMrCMSHttpHandler();
            mrCMSHttpHandler.Webpage = new Redirect {RedirectUrl = "http://www.example.com", Permanent = true};
            var httpContext = A.Fake<HttpContextBase>();

            mrCMSHttpHandler.PageIsRedirect(httpContext);
            A.CallTo(() => httpContext.Response.RedirectPermanent("http://www.example.com")).MustHaveHappened();
        }

        [Fact]
        public void MrCMSHttpHandler_PageIsRedirect_RedirectSetRedirectUrlStartsWithBackSlashItShouldNotAddExtraSlash()
        {
            var mrCMSHttpHandler = GetMrCMSHttpHandler();
            mrCMSHttpHandler.Webpage = new Redirect { RedirectUrl = "/test-redirect" };
            var httpContext = A.Fake<HttpContextBase>();

            mrCMSHttpHandler.PageIsRedirect(httpContext);
            A.CallTo(() => httpContext.Response.Redirect("~/test-redirect")).MustHaveHappened();
        }

        [Fact]
        public void MrCMSHttpHandler_GetActionName_WebpageIsNullReturnNull()
        {
            var mrCMSHttpHandler = GetMrCMSHttpHandler();
            mrCMSHttpHandler.Webpage = null;

            mrCMSHttpHandler.GetActionName().Should().BeNull();
        }

        [Fact]
        public void MrCMSHttpHandler_GetActionName_WebpageNotAllowedAndUnpublishedReturnsNull()
        {
            var mrCMSHttpHandler = GetMrCMSHttpHandler();
            mrCMSHttpHandler.Webpage = new StubDisallowedWebpage();

            mrCMSHttpHandler.GetActionName().Should().BeNull();
        }

        [Fact]
        public void MrCMSHttpHandler_GetActionName_NullDocumentTypeDefinitionReturnsNull()
        {
            var mrCMSHttpHandler = GetMrCMSHttpHandler();
            A.CallTo(() => documentService.GetDefinitionByType(typeof(StubAllowedWebpage))).Returns(null);
            mrCMSHttpHandler.Webpage = new StubAllowedWebpage { PublishOn = DateTime.UtcNow.AddDays(-1) };

            mrCMSHttpHandler.GetActionName().Should().BeNull();
            A.CallTo(() => documentService.GetDefinitionByType(typeof(StubAllowedWebpage))).MustHaveHappened();
        }

        [Fact]
        public void MrCMSHttpHandler_GetActionName_ReturnsDefinitionWebGetActionIfHttpMethodIsGET()
        {
            var mrCMSHttpHandler = GetMrCMSHttpHandler();
            var documentTypeDefinition = new DocumentTypeDefinition(ChildrenListType.WhiteList)
            {
                WebGetAction = "test-get-action"
            };
            A.CallTo(() => documentService.GetDefinitionByType(typeof(StubAllowedWebpage))).Returns(documentTypeDefinition);
            mrCMSHttpHandler.Webpage = new StubAllowedWebpage { PublishOn = DateTime.UtcNow.AddDays(-1) };
            mrCMSHttpHandler.HttpMethod = "GET";

            mrCMSHttpHandler.GetActionName().Should().Be("test-get-action");
        }

        [Fact]
        public void MrCMSHttpHandler_GetActionName_ReturnsDefinitionWebGetActionIfHttpMethodIsPOST()
        {
            var mrCMSHttpHandler = GetMrCMSHttpHandler();
            var documentTypeDefinition = new DocumentTypeDefinition(ChildrenListType.WhiteList)
            {
                WebPostAction = "test-post-action"
            };
            A.CallTo(() => documentService.GetDefinitionByType(typeof(StubAllowedWebpage))).Returns(documentTypeDefinition);
            mrCMSHttpHandler.Webpage = new StubAllowedWebpage { PublishOn = DateTime.UtcNow.AddDays(-1) };
            mrCMSHttpHandler.HttpMethod = "POST";

            mrCMSHttpHandler.GetActionName().Should().Be("test-post-action");
        }

        [Fact]
        public void MrCMSHttpHandler_GetActionName_ReturnsNullIfHttpMethodIsSomethingElse()
        {
            var mrCMSHttpHandler = GetMrCMSHttpHandler();
            var documentTypeDefinition = new DocumentTypeDefinition(ChildrenListType.WhiteList)
            {
                WebPostAction = "test-post-action"
            };
            A.CallTo(() => documentService.GetDefinitionByType(typeof(StubAllowedWebpage))).Returns(documentTypeDefinition);
            mrCMSHttpHandler.Webpage = new StubAllowedWebpage { PublishOn = DateTime.UtcNow.AddDays(-1) };
            mrCMSHttpHandler.HttpMethod = "PUT";

            mrCMSHttpHandler.GetActionName().Should().BeNull();
        }

        [Fact]
        public void MrCMSHttpHandler_CheckIsFile_UrlEndsWithoutExtensionReturnsFalse()
        {
            var mrCMSHttpHandler = GetMrCMSHttpHandler();
            A.CallTo(() => requestContext.HttpContext.Request.Url.ToString()).Returns("test-page");

            mrCMSHttpHandler.CheckIsFile().Should().BeFalse();
        }

        [Fact]
        public void MrCMSHttpHandler_CheckIsFile_UrlEndsWithExtensionReturnsTrue()
        {
            var mrCMSHttpHandler = GetMrCMSHttpHandler();
            var httpContextBase = A.Fake<HttpContextBase>();
            var httpRequestBase = A.Fake<HttpRequestBase>();
            A.CallTo(() => requestContext.HttpContext).Returns(httpContextBase);
            A.CallTo(() => httpContextBase.Request).Returns(httpRequestBase);
            A.CallTo(() => httpRequestBase.Url).Returns(new Uri("/test-page.jpg", UriKind.Relative));

            mrCMSHttpHandler.CheckIsFile().Should().BeTrue();
        }

        [Fact]
        public void MrCMSHttpHandler_SetFormData_IfHttpMethodIsPOSTAndTheFormDataIsNotNullSetTheRouteData()
        {
            var mrCMSHttpHandler = GetMrCMSHttpHandler();
            mrCMSHttpHandler.HttpMethod = "POST";
            A.CallTo(() => requestContext.HttpContext.Request.Form)
             .Returns(new NameValueCollection { { "test", "data" } });
            var controller = A.Fake<Controller>();
            var routeData = new RouteData();
            controller.ControllerContext = new ControllerContext { RouteData = routeData };
            mrCMSHttpHandler.Webpage = new StubAllowedWebpage();

            mrCMSHttpHandler.SetFormData(controller);

            routeData.Values["form"].Should().NotBeNull();
        }

        private MrCMSHttpHandler GetMrCMSHttpHandler()
        {
            requestContext = A.Fake<RequestContext>();
            session = A.Fake<ISession>();
            siteSettings = A.Fake<SiteSettings>();
            documentService = A.Fake<IDocumentService>();
            var mrCMSHttpHandler = new MrCMSHttpHandler(requestContext, () => session, () => documentService,
                                                        () => siteSettings);
            return mrCMSHttpHandler;
        }
    }
}