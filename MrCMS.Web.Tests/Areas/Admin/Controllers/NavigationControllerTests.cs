using System.Collections.Generic;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.People;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Controllers;
using MrCMS.Website;
using Xunit;

namespace MrCMS.Web.Tests.Areas.Admin.Controllers
{
    public class NavigationControllerTests
    {
        private IUserService userService;
        private INavigationService navigationService;
        private ISiteService _siteService;

        [Fact]
        public void NavigationController_WebsiteTree_ShouldReturnPartialView()
        {
            var navigationController = GetNavigationController();

            var partialViewResult = navigationController.WebSiteTree();

            partialViewResult.Should().BeOfType<PartialViewResult>();
        }

        private NavigationController GetNavigationController()
        {
            navigationService = A.Fake<INavigationService>();
            userService = A.Fake<IUserService>();
            _siteService = A.Fake<ISiteService>();
            var navigationController = new NavigationController(navigationService, userService, _siteService);
            return navigationController;
        }

        [Fact]
        public void NavigationController_WebsiteTree_ShouldReturnWebsiteTreeListModelAsModel()
        {
            var navigationController = GetNavigationController();
            var site = new Site();
            A.CallTo(() => _siteService.GetCurrentSite()).Returns(site);
            A.CallTo(() => navigationService.GetWebsiteTree(site, null)).Returns(new SiteTree<Webpage>());

            var partialViewResult = navigationController.WebSiteTree();

            partialViewResult.Model.Should().BeOfType<WebsiteTreeListModel>();
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

        [Fact]
        public void NavigationController_Navlinks_ShouldReturnAPartialViewResult()
        {
            var navigationController = GetNavigationController();

            navigationController.NavLinks().Should().BeOfType<PartialViewResult>();
        }
    }
}