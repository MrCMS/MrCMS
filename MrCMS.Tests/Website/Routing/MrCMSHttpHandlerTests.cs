using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Elmah;
using FakeItEasy;
using FluentAssertions;
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
    public class MrCMSHttpHandlerTests : MrCMSTest
    {
        private SiteSettings siteSettings;
        private IDocumentService documentService;
        private ISession session;
        private RequestContext requestContext;
        private IControllerManager controllerManager;
        private SEOSettings seoSettings;

        public MrCMSHttpHandlerTests()
        {
            CurrentRequestData.DatabaseIsInstalled = true;
            CurrentRequestData.CurrentUser = new User();
            MrCMSApplication.OverridenRootChildren = new List<Webpage>();
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

        [Fact(Skip = "Needs GetController() to be refactored to be able to run")]
        public void MrCMSHttpHandler_Handle500_CallsSiteSettings500PageId()
        {
            var mrCMSHttpHandler = GetMrCMSHttpHandler();
            var httpContext = A.Fake<HttpContextBase>();

            mrCMSHttpHandler.Handle500(httpContext, new Exception());

            A.CallTo(() => siteSettings.Error500PageId).MustHaveHappened();
        }

        [Fact(Skip = "Needs GetController() to be refactored to be able to run")]
        public void MrCMSHttpHandler_Handle500_CallsGetDocumentWithResultOf500Page()
        {
            var mrCMSHttpHandler = GetMrCMSHttpHandler();
            A.CallTo(() => siteSettings.Error500PageId).Returns(1);
            var httpContext = A.Fake<HttpContextBase>();

            mrCMSHttpHandler.Handle500(httpContext, new Exception());

            A.CallTo(() => documentService.GetDocument<Webpage>(1)).MustHaveHappened();
        }

        [Fact(Skip = "Needs GetController() to be refactored to be able to run")]
        public void MrCMSHttpHandler_Handle500_500DocumentFoundRedirectsToThatUrl()
        {
            var mrCMSHttpHandler = GetMrCMSHttpHandler();
            A.CallTo(() => siteSettings.Error500PageId).Returns(1);
            A.CallTo(() => documentService.GetDocument<Webpage>(1)).Returns(new BasicMappedWebpage { UrlSegment = "test-500" });
            var httpContext = A.Fake<HttpContextBase>();

            mrCMSHttpHandler.Handle500(httpContext, new Exception());

            A.CallTo(() => httpContext.Response.Redirect("~/test-500")).MustHaveHappened();
        }

        [Fact(Skip = "Needs GetController() to be refactored to be able to run")]
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
        public void MrCMSHttpHandler_Handle404_MustCallSiteSettings404PageId()
        {
            CurrentRequestData.ErrorSignal = new ErrorSignal();
            var mrCMSHttpHandler = GetMrCMSHttpHandler();
            A.CallTo(() => siteSettings.Error404PageId).Returns(1);
            var stubAllowedWebpage = new StubAllowedWebpage { UrlSegment = "test-404" };
            A.CallTo(() => documentService.GetDocument<Webpage>(1)).Returns(stubAllowedWebpage);
            var httpContext = A.Fake<HttpContextBase>();

            var controller = A.Fake<Controller>();
            var routeData = new RouteData();
            controller.ControllerContext = new ControllerContext { RouteData = routeData };
            A.CallTo(() => controllerManager.GetController(requestContext, stubAllowedWebpage, ""))
             .Returns(controller);


            mrCMSHttpHandler.Handle404(httpContext);

            A.CallTo(() => siteSettings.Error404PageId).MustHaveHappened();
        }

        [Fact]
        public void MrCMSHttpHandler_Handle404_MustCallDocumentServiceWithTheResultOfThe404()
        {
            CurrentRequestData.ErrorSignal = new ErrorSignal();
            var mrCMSHttpHandler = GetMrCMSHttpHandler();
            A.CallTo(() => siteSettings.Error404PageId).Returns(1);
            var stubAllowedWebpage = new StubAllowedWebpage { UrlSegment = "test-404" };
            A.CallTo(() => documentService.GetDocument<Webpage>(1)).Returns(stubAllowedWebpage);
            var httpContext = A.Fake<HttpContextBase>();

            var controller = A.Fake<Controller>();
            var routeData = new RouteData();
            controller.ControllerContext = new ControllerContext { RouteData = routeData };
            A.CallTo(() => controllerManager.GetController(requestContext, stubAllowedWebpage, ""))
             .Returns(controller);


            mrCMSHttpHandler.Handle404(httpContext);

            A.CallTo(() => documentService.GetDocument<Webpage>(1)).MustHaveHappened();
        }

        [Fact]
        public void MrCMSHttpHandler_Handle404_StatusCode404()
        {
            CurrentRequestData.ErrorSignal = new ErrorSignal();
            var mrCMSHttpHandler = GetMrCMSHttpHandler();
            A.CallTo(() => siteSettings.Error404PageId).Returns(1);
            var stubAllowedWebpage = new StubAllowedWebpage { UrlSegment = "test-404" };
            A.CallTo(() => documentService.GetDocument<Webpage>(1)).Returns(stubAllowedWebpage);
            var httpContext = A.Fake<HttpContextBase>();

            var controller = A.Fake<Controller>();
            var routeData = new RouteData();
            controller.ControllerContext = new ControllerContext { RouteData = routeData };
            A.CallTo(() => controllerManager.GetController(requestContext, stubAllowedWebpage, ""))
             .Returns(controller);

            mrCMSHttpHandler.Handle404(httpContext);

            httpContext.Response.StatusCode.Should().Be(404);
        }

        [Fact]
        public void MrCMSHttpHandler_Handle404_WebpageSetReturnsFalse()
        {
            CurrentRequestData.ErrorSignal = new ErrorSignal();
            var mrCMSHttpHandler = GetMrCMSHttpHandler();
            mrCMSHttpHandler.Webpage = new BasicMappedWebpage {PublishOn = DateTime.Today.AddDays(-1)};
            var stubAllowedWebpage = new StubAllowedWebpage { UrlSegment = "test-404",PublishOn = DateTime.Today.AddDays(-1)};
            var httpContext = A.Fake<HttpContextBase>();
            mrCMSHttpHandler.Webpage = stubAllowedWebpage;


            mrCMSHttpHandler.Handle404(httpContext).Should().BeFalse();
        }

        [Fact(Skip = "Invalid logic - should not redirect")]
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
            mrCMSHttpHandler.Webpage = new Redirect { RedirectUrl = "test-redirect", Permanent = true };
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
            mrCMSHttpHandler.Webpage = new Redirect { RedirectUrl = "http://www.example.com", Permanent = true };
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
        public void MrCMSHttpHandler_CheckIsFile_UrlEndsWithoutExtensionReturnsFalse()
        {
            var mrCMSHttpHandler = GetMrCMSHttpHandler();
            A.CallTo(() => requestContext.HttpContext.Request.Url).Returns(new Uri("http://www.example.com/test-page"));

            mrCMSHttpHandler.CheckIsFile(CurrentRequestData.CurrentContext).Should().BeFalse();
        }

        [Fact]
        public void MrCMSHttpHandler_CheckIsFile_UrlEndsWithExtensionReturnsTrue()
        {
            var mrCMSHttpHandler = GetMrCMSHttpHandler();
            A.CallTo(() => requestContext.HttpContext.Request.Url).Returns(new Uri("http://www.example.com/test-page.jpg"));

            mrCMSHttpHandler.CheckIsFile(CurrentRequestData.CurrentContext).Should().BeTrue();
        }


        private MrCMSHttpHandler GetMrCMSHttpHandler()
        {
            requestContext = A.Fake<RequestContext>();
            session = A.Fake<ISession>();
            siteSettings = A.Fake<SiteSettings>();
            seoSettings = A.Fake<SEOSettings>();
            documentService = A.Fake<IDocumentService>();
            controllerManager = A.Fake<IControllerManager>();
            var mrCMSHttpHandler = new MrCMSHttpHandler(session, documentService, controllerManager, siteSettings,seoSettings);
            mrCMSHttpHandler.SetRequestContext(requestContext);
            return mrCMSHttpHandler;
        }
    }
}