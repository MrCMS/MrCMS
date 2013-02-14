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
        [Fact(Skip = "In the process of refactoring this")]
        public void MrCMSRouteHandler_GetHttpHandler_ReturnsMrCMSHttpHandler()
        {
            IRouteHandler mrCMSRouteHandler = GetMrCMSRouteHandler();

            mrCMSRouteHandler.GetHttpHandler(A.Fake<RequestContext>()).Should().BeOfType<MrCMSHttpHandler>();
        }

        private MrCMSRouteHandler GetMrCMSRouteHandler()
        {
            return new MrCMSRouteHandler();
        }
    }
}