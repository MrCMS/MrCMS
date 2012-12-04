using System;
using System.Web;
using System.Web.Routing;
using FakeItEasy;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Website.Routing;
using Xunit;
using FluentAssertions;

namespace MrCMS.Tests.Website.Routing
{
    public class MrCMSRouteHandlerTests
    {
        [Fact]
        public void MrCMSRouteHandler_GetHttpHandler_ReturnsMrCMSHttpHandler()
        {
            IRouteHandler mrCMSRouteHandler = new MrCMSRouteHandler(null, null);

            mrCMSRouteHandler.GetHttpHandler(A.Fake<RequestContext>()).Should().BeOfType<MrCMSHttpHandler>();
        }

        [Fact]
        public void MrCMSRouteHandler_GetHttpHandler_DocumentServiceAndSiteSettingsShouldBeSetFromConstructorArguments()
        {
            Func<IDocumentService> documentService = A.Fake<Func<IDocumentService>>();
            Func<SiteSettings> siteSettings = A.Fake<Func<SiteSettings>>();
            IRouteHandler mrCMSRouteHandler = new MrCMSRouteHandler(documentService, siteSettings);

            var mrCMSHttpHandler = mrCMSRouteHandler.GetHttpHandler(A.Fake<RequestContext>()).As<MrCMSHttpHandler>();

            mrCMSHttpHandler.GetDocumentService.Should().Be(documentService);
            mrCMSHttpHandler.GetSiteSettings.Should().Be(siteSettings);
        }
    }
}