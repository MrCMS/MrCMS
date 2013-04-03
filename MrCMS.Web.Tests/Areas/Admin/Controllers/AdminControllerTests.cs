using System.Text;
using System.Web;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Web.Areas.Admin.Controllers;
using MrCMS.Website.ActionResults;
using MrCMS.Website.Controllers;
using Xunit;

namespace MrCMS.Web.Tests.Areas.Admin.Controllers
{
    public class AdminControllerTests
    {
        [Fact]
        public void AdminController_Json_ObjectStringEncodingReturnsJsonNetResult()
        {
            var controller = new StubMrCMSAdminController();

            var data = new object();
            JsonResult jsonResult = controller.Json(data, "test/content-type", Encoding.ASCII);

            jsonResult.Should().BeOfType<JsonNetResult>();
        }

        [Fact]
        public void AdminController_Request_IfRequestMockIsSetReturnsThat()
        {
            var controller = new StubMrCMSAdminController();

            var httpRequestBase = A.Fake<HttpRequestBase>();
            controller.RequestMock = httpRequestBase;

            controller.Request.Should().Be(httpRequestBase);
        }
    }

    public class StubMrCMSAdminController : MrCMSAdminController
    {
        public new JsonResult Json(object data, string contentType, Encoding contentEncoding)
        {
            return base.Json(data, contentType, contentEncoding);
        }
    }
}