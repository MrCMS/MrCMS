using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.People;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Tests;
using MrCMS.Tests.Services;
using MrCMS.Web.Areas.Admin.Controllers;
using Xunit;
using MrCMS.Helpers;

namespace MrCMS.Web.Tests.Areas.Admin.Controllers
{
    public class NavigationControllerTests
    {
        private static IUserService userService;
        private static INavigationService navigationService;

        [Fact]
        public void NavigationController_WebsiteTree_ShouldReturnPartialView()
        {
            var navigationController = GetNavigationController();

            var partialViewResult = navigationController.WebSiteTree();

            partialViewResult.Should().BeOfType<PartialViewResult>();
        }

        private static NavigationController GetNavigationController()
        {
            navigationService = A.Fake<INavigationService>();
            userService = A.Fake<IUserService>();
            var navigationController = new NavigationController(navigationService, userService);
            return navigationController;
        }

        [Fact]
        public void NavigationController_WebsiteTree_ShouldReturnSiteTreeAsModel()
        {
            var navigationController = GetNavigationController();
            A.CallTo(() => navigationService.GetWebsiteTree(null)).Returns(new SiteTree<Webpage>());

            var partialViewResult = navigationController.WebSiteTree();

            partialViewResult.Model.Should().BeOfType<SiteTree<Webpage>>();
        }

        [Fact]
        public void NavigationController_LayoutTree_ShouldReturnSiteTreeAsModel()
        {
            var navigationController = GetNavigationController();
            A.CallTo(() => navigationService.GetLayoutList()).Returns(new SiteTree<Layout>());

            var partialViewResult = navigationController.LayoutTree();

            partialViewResult.Model.Should().BeOfType<SiteTree<Layout>>();
        }

        [Fact]
        public void NavigationController_MediaTree_ShouldReturnSiteTreeAsModel()
        {
            var navigationController = GetNavigationController();
            A.CallTo(() => navigationService.GetMediaTree()).Returns(new SiteTree<MediaCategory>());

            var partialViewResult = navigationController.MediaTree();

            partialViewResult.Model.Should().BeOfType<SiteTree<MediaCategory>>();
        }

        [Fact]
        public void NavigationController_UserList_ShouldCallUserList()
        {
            var navigationController = GetNavigationController();

            navigationController.UserList();

            A.CallTo(() => navigationService.GetUserList()).MustHaveHappened();
        }

        [Fact]
        public void NavigationController_UserList_ShouldReturnAPartialViewResult()
        {
            var navigationController = GetNavigationController();

            var result = navigationController.UserList();

            result.Should().NotBeNull();
        }

        [Fact]
        public void NavigationController_UserList_ModelShouldBeResponseOfNavigationService()
        {
            var navigationController = GetNavigationController();
            var siteTree = new SiteTree<User>();
            A.CallTo(() => navigationService.GetUserList()).Returns(siteTree);

            var result = navigationController.UserList();

            result.Model.Should().Be(siteTree);
        }


        [Fact]
        public void NavigationController_LoggedInAs_ShouldCallGetEmailByName()
        {
            var navigationController = GetNavigationController();
            navigationController.LoggedInAs();
        }
    }
}