using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Documents;
using MrCMS.Tests.Stubs;
using MrCMS.Website;
using MrCMS.Website.Routing;
using Xunit;
using MrCMS.Helpers;

namespace MrCMS.Tests.Website.Routing
{
    public class ControllerManagerTests : MrCMSTest
    {
        [Fact]
        public void ControllerManager_GetControllerName_NullWebpageReturnsNull()
        {
            var controllerManager = GetControllerManager();

            controllerManager.GetControllerName(null, "GET").Should().BeNull();
        }

        [Fact]
        public void ControllerManager_GetControllerName_WebpageNotPublishedAndNotAllowedReturnsNull()
        {
            var controllerManager = GetControllerManager();

            controllerManager.GetControllerName(new StubDisallowedWebpage(), "GET").Should().BeNull();
        }

        [Fact]
        public void ControllerManager_GetControllerName_NullDocumentMetadataReturnsNull()
        {
            var controllerManager = GetControllerManager();
            var stubAllowedWebpage = new StubAllowedWebpage { PublishOn = CurrentRequestData.Now.AddDays(-1) };
            controllerManager.GetMetadata = document => null;

            controllerManager.GetControllerName(stubAllowedWebpage, "GET").Should().BeNull();
        }

        [Fact]
        public void ControllerManager_GetControllerName_HttpMethodIsGETReturnsWebGetController()
        {
            var controllerManager = GetControllerManager();
            var metadata = new DocumentMetadata
                                             {
                                                 WebGetController = "test-controller"
                                             };
            var stubAllowedWebpage = new StubAllowedWebpage { PublishOn = CurrentRequestData.Now.AddDays(-1) };
            controllerManager.GetMetadata = document => metadata;

            controllerManager.GetControllerName(stubAllowedWebpage, "GET").Should().Be("test-controller");
        }

        [Fact]
        public void ControllerManager_GetControllerName_HttpMethodIsPOSTReturnsWebGetController()
        {
            var controllerManager = GetControllerManager();
            var metadata = new DocumentMetadata
                                             {
                                                 WebPostController = "test-controller"
                                             };
            var stubAllowedWebpage = new StubAllowedWebpage { PublishOn = CurrentRequestData.Now.AddDays(-1) };
            controllerManager.GetMetadata = document => metadata;

            controllerManager.GetControllerName(stubAllowedWebpage, "POST").Should().Be("test-controller");
        }

        [Fact]
        public void ControllerManager_GetControllerName_HttpMethodIsAnotherTypeReturnsNull()
        {
            var controllerManager = GetControllerManager();
            var metadata = new DocumentMetadata
                                             {
                                                 WebPostController = "test-controller"
                                             };
            var stubAllowedWebpage = new StubAllowedWebpage { PublishOn = CurrentRequestData.Now.AddDays(-1) };
            controllerManager.GetMetadata = document => metadata;


            controllerManager.GetControllerName(stubAllowedWebpage, "PUT").Should().BeNull();
        }


        [Fact]
        public void ControllerManager_GetActionName_WebpageIsNullReturnNull()
        {
            var controllerManager = GetControllerManager();

            controllerManager.GetActionName(null, "GET").Should().BeNull();
        }

        [Fact]
        public void ControllerManager_GetActionName_WebpageNotAllowedAndUnpublishedReturnsNull()
        {
            var controllerManager = GetControllerManager();
            var stubDisallowedWebpage = new StubDisallowedWebpage();

            controllerManager.GetActionName(stubDisallowedWebpage, "GET").Should().BeNull();
        }

        [Fact]
        public void ControllerManager_GetActionName_NullDocumentMetadataReturnsNull()
        {
            var controllerManager = GetControllerManager();
            var stubAllowedWebpage = new StubAllowedWebpage { PublishOn = CurrentRequestData.Now.AddDays(-1) };
            controllerManager.GetMetadata = document => null;

            controllerManager.GetActionName(stubAllowedWebpage, "GET").Should().BeNull();
        }

        [Fact]
        public void ControllerManager_GetActionName_ReturnsDefinitionWebGetActionIfHttpMethodIsGET()
        {
            var controllerManager = GetControllerManager();
            var metadata = new DocumentMetadata
            {
                WebGetAction = "test-get-action"
            };
            var webpage = new StubAllowedWebpage { PublishOn = CurrentRequestData.Now.AddDays(-1) };

            controllerManager.GetMetadata = document => metadata;

            controllerManager.GetActionName(webpage, "GET").Should().Be("test-get-action");
        }

        [Fact]
        public void ControllerManager_GetActionName_ReturnsDefinitionWebGetActionIfHttpMethodIsPOST()
        {
            var controllerManager = GetControllerManager();
            var metadata = new DocumentMetadata
            {
                WebPostAction = "test-post-action"
            };
            var webpage = new StubAllowedWebpage { PublishOn = CurrentRequestData.Now.AddDays(-1) };

            controllerManager.GetMetadata = document => metadata;

            controllerManager.GetActionName(webpage, "POST").Should().Be("test-post-action");
        }

        [Fact]
        public void ControllerManager_GetActionName_ReturnsNullIfHttpMethodIsSomethingElse()
        {
            var controllerManager = GetControllerManager();
            var metadata = new DocumentMetadata
            {
                WebPostAction = "test-post-action"
            };
            var webpage = new StubAllowedWebpage { PublishOn = CurrentRequestData.Now.AddDays(-1) };

            controllerManager.GetMetadata = document => metadata;

            controllerManager.GetActionName(webpage, "PUT").Should().BeNull();
        }

        [Fact]
        public void ControllerManager_SetFormData_IfTheFormDataIsNotNullSetTheRouteData()
        {
            var controllerManager = GetControllerManager();
            var nameValueCollection = new NameValueCollection { { "test", "data" } };
            var controller = A.Fake<Controller>();
            var routeData = new RouteData();
            controller.ControllerContext = new ControllerContext { RouteData = routeData };
            var webpage = new StubAllowedWebpage();

            controllerManager.SetFormData(webpage, controller, nameValueCollection);

            routeData.Values["form"].Should().NotBeNull();
        }

        ControllerManager GetControllerManager()
        {
            return new ControllerManager();
        }
    }
}