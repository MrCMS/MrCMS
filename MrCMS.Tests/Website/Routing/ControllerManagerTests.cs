using System.Collections.Specialized;
using System.Web.Mvc;
using System.Web.Routing;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Documents;
using MrCMS.Entities.People;
using MrCMS.Services;
using MrCMS.Tests.Stubs;
using MrCMS.Website;
using MrCMS.Website.Routing;
using Xunit;

namespace MrCMS.Tests.Website.Routing
{
    public class ControllerManagerTests : MrCMSTest
    {
        private readonly ControllerManager _controllerManager;
        private readonly IUserUIPermissionsService _userUIPermissionsService;
        private readonly IGetCurrentUser _getCurrentUser;
        private User _user;

        public ControllerManagerTests()
        {
            _userUIPermissionsService = A.Fake<IUserUIPermissionsService>();
            _controllerManager = new ControllerManager(_userUIPermissionsService);
        }

        [Fact]
        public void ControllerManager_GetControllerName_NullWebpageReturnsNull()
        {
            _controllerManager.GetControllerName(null, "GET").Should().BeNull();
        }

        [Fact]
        public void ControllerManager_GetControllerName_WebpageNotPublishedAndNotAllowedReturnsNull()
        {
            var stubDisallowedWebpage = new StubDisallowedWebpage();
            A.CallTo(() => _userUIPermissionsService.IsCurrentUserAllowed(stubDisallowedWebpage)).Returns(false);
            _controllerManager.GetControllerName(stubDisallowedWebpage, "GET").Should().BeNull();
        }

        [Fact]
        public void ControllerManager_GetControllerName_NullDocumentMetadataReturnsNull()
        {
            var stubAllowedWebpage = new StubAllowedWebpage { PublishOn = CurrentRequestData.Now.AddDays(-1) };
            _controllerManager.GetMetadata = document => null;

            _controllerManager.GetControllerName(stubAllowedWebpage, "GET").Should().BeNull();
        }

        [Fact]
        public void ControllerManager_GetControllerName_HttpMethodIsGETReturnsWebGetController()
        {
            var metadata = new DocumentMetadata
            {
                WebGetController = "test-controller"
            };
            var stubAllowedWebpage = new StubAllowedWebpage { PublishOn = CurrentRequestData.Now.AddDays(-1) };
            _controllerManager.GetMetadata = document => metadata;

            _controllerManager.GetControllerName(stubAllowedWebpage, "GET").Should().Be("test-controller");
        }

        [Fact]
        public void ControllerManager_GetControllerName_HttpMethodIsPOSTReturnsWebGetController()
        {
            var metadata = new DocumentMetadata
            {
                WebPostController = "test-controller"
            };
            var stubAllowedWebpage = new StubAllowedWebpage { PublishOn = CurrentRequestData.Now.AddDays(-1) };
            _controllerManager.GetMetadata = document => metadata;

            _controllerManager.GetControllerName(stubAllowedWebpage, "POST").Should().Be("test-controller");
        }

        [Fact]
        public void ControllerManager_GetControllerName_HttpMethodIsAnotherTypeReturnsNull()
        {
            var metadata = new DocumentMetadata
            {
                WebPostController = "test-controller"
            };
            var stubAllowedWebpage = new StubAllowedWebpage { PublishOn = CurrentRequestData.Now.AddDays(-1) };
            _controllerManager.GetMetadata = document => metadata;


            _controllerManager.GetControllerName(stubAllowedWebpage, "PUT").Should().BeNull();
        }


        [Fact]
        public void ControllerManager_GetActionName_WebpageIsNullReturnNull()
        {
            _controllerManager.GetActionName(null, "GET").Should().BeNull();
        }

        [Fact]
        public void ControllerManager_GetActionName_WebpageNotAllowedAndUnpublishedReturnsNull()
        {
            var stubDisallowedWebpage = new StubDisallowedWebpage();

            A.CallTo(() => _userUIPermissionsService.IsCurrentUserAllowed(stubDisallowedWebpage)).Returns(false);

            _controllerManager.GetActionName(stubDisallowedWebpage, "GET").Should().BeNull();
        }

        [Fact]
        public void ControllerManager_GetActionName_NullDocumentMetadataReturnsNull()
        {
            var stubAllowedWebpage = new StubAllowedWebpage { PublishOn = CurrentRequestData.Now.AddDays(-1) };
            _controllerManager.GetMetadata = document => null;

            _controllerManager.GetActionName(stubAllowedWebpage, "GET").Should().BeNull();
        }

        [Fact]
        public void ControllerManager_GetActionName_ReturnsDefinitionWebGetActionIfHttpMethodIsGET()
        {
            var metadata = new DocumentMetadata
            {
                WebGetAction = "test-get-action"
            };
            var webpage = new StubAllowedWebpage { PublishOn = CurrentRequestData.Now.AddDays(-1) };

            _controllerManager.GetMetadata = document => metadata;

            _controllerManager.GetActionName(webpage, "GET").Should().Be("test-get-action");
        }

        [Fact]
        public void ControllerManager_GetActionName_ReturnsDefinitionWebGetActionIfHttpMethodIsPOST()
        {
            var metadata = new DocumentMetadata
            {
                WebPostAction = "test-post-action"
            };
            var webpage = new StubAllowedWebpage { PublishOn = CurrentRequestData.Now.AddDays(-1) };

            _controllerManager.GetMetadata = document => metadata;

            _controllerManager.GetActionName(webpage, "POST").Should().Be("test-post-action");
        }

        [Fact]
        public void ControllerManager_GetActionName_ReturnsNullIfHttpMethodIsSomethingElse()
        {
            var metadata = new DocumentMetadata
            {
                WebPostAction = "test-post-action"
            };
            var webpage = new StubAllowedWebpage { PublishOn = CurrentRequestData.Now.AddDays(-1) };

            _controllerManager.GetMetadata = document => metadata;

            _controllerManager.GetActionName(webpage, "PUT").Should().BeNull();
        }

        [Fact]
        public void ControllerManager_SetFormData_IfTheFormDataIsNotNullSetTheRouteData()
        {
            var nameValueCollection = new NameValueCollection { { "test", "data" } };
            var controller = A.Fake<Controller>();
            var routeData = new RouteData();
            controller.ControllerContext = new ControllerContext { RouteData = routeData };
            var webpage = new StubAllowedWebpage();

            _controllerManager.SetFormData(webpage, controller, nameValueCollection);

            routeData.Values["form"].Should().NotBeNull();
        }
    }
}