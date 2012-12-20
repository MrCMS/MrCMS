using System;
using System.Web;
using System.Web.Routing;
using FakeItEasy;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Website.Routing;
using NHibernate;
using Xunit;
using FluentAssertions;

namespace MrCMS.Tests.Website.Routing
{
    public class MrCMSRouteHandlerTests
    {
        private static Func<ISession> session;
        private static Func<IDocumentService> documentService;
        private static Func<SiteSettings> siteSettings;

        [Fact]
        public void MrCMSRouteHandler_GetHttpHandler_ReturnsMrCMSHttpHandler()
        {
            IRouteHandler mrCMSRouteHandler = GetMrCMSRouteHandler();

            mrCMSRouteHandler.GetHttpHandler(A.Fake<RequestContext>()).Should().BeOfType<MrCMSHttpHandler>();
        }

        [Fact]
        public void MrCMSRouteHandler_GetHttpHandler_DocumentServiceAndSiteSettingsShouldBeSetFromConstructorArguments()
        {
            IRouteHandler mrCMSRouteHandler = GetMrCMSRouteHandler();

            var mrCMSHttpHandler = mrCMSRouteHandler.GetHttpHandler(A.Fake<RequestContext>()).As<MrCMSHttpHandler>();

            mrCMSHttpHandler.GetDocumentService.Should().Be(documentService);
            mrCMSHttpHandler.GetSiteSettings.Should().Be(siteSettings);
        }

        private static MrCMSRouteHandler GetMrCMSRouteHandler()
        {
            session = A.Fake<Func<ISession>>();
            documentService = A.Fake<Func<IDocumentService>>();
            siteSettings = A.Fake<Func<SiteSettings>>();
            return new MrCMSRouteHandler(session, documentService, siteSettings);
        }
    }
}