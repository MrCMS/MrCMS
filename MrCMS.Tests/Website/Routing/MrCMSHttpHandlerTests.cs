using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
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
            MrCMSApplication.DatabaseIsInstalled = true;
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
            MrCMSApplication.DatabaseIsInstalled = true;
            MrCMSApplication.OverriddenUser = new User();
            var mrCMSHttpHandler = new MrCMSHttpHandler(A.Fake<RequestContext>(), null, null);
            var httpContext = A.Fake<HttpContextBase>();
            mrCMSHttpHandler.Webpage = new StubAllowedWebpage();

            mrCMSHttpHandler.IsAllowed(httpContext).Should().BeTrue();
        }

        [Fact]
        public void MrCMSHttpHandler_IsAllowed_ReturnsFalseIfCurrentUserIsDisallowedForWebpage()
        {
            MrCMSApplication.DatabaseIsInstalled = true;
            MrCMSApplication.OverriddenUser = new User();
            var mrCMSHttpHandler = new MrCMSHttpHandler(A.Fake<RequestContext>(), null, null);
            var httpContext = A.Fake<HttpContextBase>();
            mrCMSHttpHandler.Webpage = new StubDisallowedWebpage();

            mrCMSHttpHandler.IsAllowed(httpContext).Should().BeFalse();
        }

        [Fact]
        public void MrCMSHttpHandler_IsAllowed_ShouldRedirectToRootIfDisallowed()
        {
            MrCMSApplication.DatabaseIsInstalled = true;
            MrCMSApplication.OverriddenUser = new User();
            var mrCMSHttpHandler = new MrCMSHttpHandler(A.Fake<RequestContext>(), null, null);
            var httpContext = A.Fake<HttpContextBase>();
            mrCMSHttpHandler.Webpage = new StubDisallowedWebpage();

            mrCMSHttpHandler.IsAllowed(httpContext);

            A.CallTo(() => httpContext.Response.Redirect("~")).MustHaveHappened();
        }

        [Fact]
        public void MrCMSHttpHandler_Handle500_CallsSiteSettings500PageId()
        {
            MrCMSApplication.DatabaseIsInstalled = true;
            MrCMSApplication.OverriddenUser = new User();
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
            MrCMSApplication.DatabaseIsInstalled = true;
            MrCMSApplication.OverriddenUser = new User();
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
            MrCMSApplication.DatabaseIsInstalled = true;
            MrCMSApplication.OverriddenSession = A.Fake<ISession>();
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
            MrCMSApplication.DatabaseIsInstalled = true;
            MrCMSApplication.OverriddenSession = A.Fake<ISession>();
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
            MrCMSApplication.OverriddenUser = new User();
            MrCMSApplication.OverriddenSession = A.Fake<ISession>();
            MrCMSApplication.OverridenContext = A.Fake<HttpContextBase>();
            var mrCMSHttpHandler = new MrCMSHttpHandler(A.Fake<RequestContext>(), null, null)
                                       {
                                           Webpage = null
                                       };

            mrCMSHttpHandler.GetControllerName().Should().BeNull();
        }

        [Fact]
        public void MrCMSHttpHandler_GetControllerName_WebpageNotPublishedAndNotAllowedReturnsNull()
        {
            MrCMSApplication.OverriddenUser = new User();
            MrCMSApplication.OverriddenSession = A.Fake<ISession>();
            MrCMSApplication.OverridenContext = A.Fake<HttpContextBase>();
            var mrCMSHttpHandler = new MrCMSHttpHandler(A.Fake<RequestContext>(), null, null)
            {
                Webpage = new StubDisallowedWebpage()
            };

            mrCMSHttpHandler.GetControllerName().Should().BeNull();
        }

        [Fact]
        public void MrCMSHttpHandler_GetControllerName_NullDocumentTypeDefinitionReturnsNull()
        {
            MrCMSApplication.OverriddenUser = new User();
            MrCMSApplication.OverriddenSession = A.Fake<ISession>();
            MrCMSApplication.OverridenContext = A.Fake<HttpContextBase>();
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
            MrCMSApplication.OverriddenUser = new User();
            MrCMSApplication.OverriddenSession = A.Fake<ISession>();
            MrCMSApplication.OverridenContext = A.Fake<HttpContextBase>();
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
            MrCMSApplication.OverriddenUser = new User();
            MrCMSApplication.OverriddenSession = A.Fake<ISession>();
            MrCMSApplication.OverridenContext = A.Fake<HttpContextBase>();
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
            MrCMSApplication.OverriddenUser = new User();
            MrCMSApplication.OverriddenSession = A.Fake<ISession>();
            MrCMSApplication.OverridenContext = A.Fake<HttpContextBase>();
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
            MrCMSApplication.DatabaseIsInstalled = true;
            MrCMSApplication.OverriddenUser = new User();
            MrCMSApplication.OverriddenSession = A.Fake<ISession>();
            MrCMSApplication.OverridenContext = A.Fake<HttpContextBase>();
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
            MrCMSApplication.DatabaseIsInstalled = true;
            MrCMSApplication.OverriddenUser = new User();
            MrCMSApplication.OverriddenSession = A.Fake<ISession>();
            MrCMSApplication.OverridenContext = A.Fake<HttpContextBase>();
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
            MrCMSApplication.DatabaseIsInstalled = true;
            MrCMSApplication.OverriddenUser = new User();
            MrCMSApplication.OverriddenSession = A.Fake<ISession>();
            MrCMSApplication.OverridenContext = A.Fake<HttpContextBase>();
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
            MrCMSApplication.DatabaseIsInstalled = true;
            MrCMSApplication.OverriddenUser = new User();
            MrCMSApplication.OverriddenSession = A.Fake<ISession>();
            MrCMSApplication.OverridenContext = A.Fake<HttpContextBase>();
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
            MrCMSApplication.DatabaseIsInstalled = true;
            MrCMSApplication.OverriddenUser = new User();
            MrCMSApplication.OverriddenSession = A.Fake<ISession>();
            MrCMSApplication.OverridenContext = A.Fake<HttpContextBase>();
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
        public void MrCMSHttpHandler_PageIsRedirect_NullWebpageReturnsFalse()
        {
            MrCMSApplication.DatabaseIsInstalled = true;
            MrCMSApplication.OverriddenUser = new User();
            MrCMSApplication.OverriddenSession = A.Fake<ISession>();
            MrCMSApplication.OverridenContext = A.Fake<HttpContextBase>();
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
            MrCMSApplication.DatabaseIsInstalled = true;
            MrCMSApplication.OverriddenUser = new User();
            MrCMSApplication.OverriddenSession = A.Fake<ISession>();
            MrCMSApplication.OverridenContext = A.Fake<HttpContextBase>();
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
            MrCMSApplication.DatabaseIsInstalled = true;
            MrCMSApplication.OverriddenUser = new User();
            MrCMSApplication.OverriddenSession = A.Fake<ISession>();
            MrCMSApplication.OverridenContext = A.Fake<HttpContextBase>();
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
            MrCMSApplication.DatabaseIsInstalled = true;
            MrCMSApplication.OverriddenUser = new User();
            MrCMSApplication.OverriddenSession = A.Fake<ISession>();
            MrCMSApplication.OverridenContext = A.Fake<HttpContextBase>();
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
            MrCMSApplication.DatabaseIsInstalled = true;
            MrCMSApplication.OverriddenUser = new User();
            MrCMSApplication.OverriddenSession = A.Fake<ISession>();
            MrCMSApplication.OverridenContext = A.Fake<HttpContextBase>();
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
            MrCMSApplication.DatabaseIsInstalled = true;
            MrCMSApplication.OverriddenUser = new User();
            MrCMSApplication.OverriddenSession = A.Fake<ISession>();
            MrCMSApplication.OverridenContext = A.Fake<HttpContextBase>();
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
            MrCMSApplication.DatabaseIsInstalled = true;
            MrCMSApplication.OverriddenUser = new User();
            MrCMSApplication.OverriddenSession = A.Fake<ISession>();
            MrCMSApplication.OverridenContext = A.Fake<HttpContextBase>();
            var mrCMSHttpHandler = new MrCMSHttpHandler(A.Fake<RequestContext>(), null, null)
            {
                Webpage = null
            };

            mrCMSHttpHandler.GetActionName().Should().BeNull();
        }

        [Fact]
        public void MrCMSHttpHandler_GetActionName_WebpageNotAllowedAndUnpublishedReturnsNull()
        {
            MrCMSApplication.DatabaseIsInstalled = true;
            MrCMSApplication.OverriddenUser = new User();
            MrCMSApplication.OverriddenSession = A.Fake<ISession>();
            MrCMSApplication.OverridenContext = A.Fake<HttpContextBase>();
            var mrCMSHttpHandler = new MrCMSHttpHandler(A.Fake<RequestContext>(), null, null)
            {
                Webpage = new StubDisallowedWebpage()
            };

            mrCMSHttpHandler.GetActionName().Should().BeNull();
        }

        [Fact]
        public void MrCMSHttpHandler_GetActionName_NullDocumentTypeDefinitionReturnsNull()
        {
            MrCMSApplication.DatabaseIsInstalled = true;
            MrCMSApplication.OverriddenUser = new User();
            MrCMSApplication.OverriddenSession = A.Fake<ISession>();
            MrCMSApplication.OverridenContext = A.Fake<HttpContextBase>();
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
            MrCMSApplication.DatabaseIsInstalled = true;
            MrCMSApplication.OverriddenUser = new User();
            MrCMSApplication.OverriddenSession = A.Fake<ISession>();
            MrCMSApplication.OverridenContext = A.Fake<HttpContextBase>();
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
            MrCMSApplication.DatabaseIsInstalled = true;
            MrCMSApplication.OverriddenUser = new User();
            MrCMSApplication.OverriddenSession = A.Fake<ISession>();
            MrCMSApplication.OverridenContext = A.Fake<HttpContextBase>();
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
            MrCMSApplication.DatabaseIsInstalled = true;
            MrCMSApplication.OverriddenUser = new User();
            MrCMSApplication.OverriddenSession = A.Fake<ISession>();
            MrCMSApplication.OverridenContext = A.Fake<HttpContextBase>();
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
            MrCMSApplication.DatabaseIsInstalled = true;
            MrCMSApplication.OverriddenUser = new User();
            MrCMSApplication.OverriddenSession = A.Fake<ISession>();
            MrCMSApplication.OverridenContext = A.Fake<HttpContextBase>();
            var requestContext = A.Fake<RequestContext>();
            A.CallTo(() => requestContext.HttpContext.Request.Url.ToString()).Returns("test-page");
            var mrCMSHttpHandler = new MrCMSHttpHandler(requestContext, null, null);

            mrCMSHttpHandler.CheckIsFile().Should().BeFalse();
        }

        [Fact]
        public void MrCMSHttpHandler_CheckIsFile_UrlEndsWithExtensionReturnsTrue()
        {
            MrCMSApplication.DatabaseIsInstalled = true;
            MrCMSApplication.OverriddenUser = new User();
            MrCMSApplication.OverriddenSession = A.Fake<ISession>();
            MrCMSApplication.OverridenContext = A.Fake<HttpContextBase>();
            var requestContext = A.Fake<RequestContext>();
            A.CallTo(() => requestContext.HttpContext.Request.Url.ToString()).Returns("test-page.jpg");
            var mrCMSHttpHandler = new MrCMSHttpHandler(requestContext, null, null);

            mrCMSHttpHandler.CheckIsFile().Should().BeTrue();
        }

        [Fact]
        public void MrCMSHttpHandler_SetFormData_IfHttpMethodIsPOSTAndTheFormDataIsNotNullSetTheRouteData()
        {
            MrCMSApplication.DatabaseIsInstalled = true;
            MrCMSApplication.OverriddenUser = new User();
            MrCMSApplication.OverriddenSession = A.Fake<ISession>();
            MrCMSApplication.OverridenContext = A.Fake<HttpContextBase>();
            var requestContext = A.Fake<RequestContext>();
            A.CallTo(() => requestContext.HttpContext.Request.Form)
             .Returns(new NameValueCollection {{"test", "data"}});
            var mrCMSHttpHandler = new MrCMSHttpHandler(requestContext, null, null) {HttpMethod = "POST"};

            var controller = A.Fake<Controller>();
            var routeData = new RouteData();
            controller.ControllerContext = new ControllerContext {RouteData = routeData};
            mrCMSHttpHandler.SetFormData(controller);

            routeData.Values["form"].Should().NotBeNull();
        }
    }
}