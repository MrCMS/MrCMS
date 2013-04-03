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
using Xunit;

namespace MrCMS.Web.Tests.Areas.Admin.Controllers
{
    public class NavigationControllerTests
    {
        private ISiteService _siteService;
        private INavigationService navigationService;

        [Fact]
        public void NavigationController_WebsiteTree_ShouldReturnPartialView()
        {
            NavigationController navigationController = GetNavigationController();

            PartialViewResult partialViewResult = navigationController.WebSiteTree();

            partialViewResult.Should().BeOfType<PartialViewResult>();
        }

        private NavigationController GetNavigationController()
        {
            navigationService = A.Fake<INavigationService>();
            _siteService = A.Fake<ISiteService>();
            var navigationController = new NavigationController(navigationService,  _siteService);
            return navigationController;
        }

        [Fact]
        public void NavigationController_WebsiteTree_ShouldReturnWebsiteTreeAsModel()
        {
            NavigationController navigationController = GetNavigationController();
            var site = new Site();
            A.CallTo(() => _siteService.GetCurrentSite()).Returns(site);
            A.CallTo(() => navigationService.GetWebsiteTree(null)).Returns(new SiteTree<Webpage>());

            PartialViewResult partialViewResult = navigationController.WebSiteTree();

            partialViewResult.Model.Should().BeOfType<SiteTree<Webpage>>();
        }

        [Fact]
        public void NavigationController_LayoutTree_ShouldReturnLayoutTreeAsModel()
        {
            NavigationController navigationController = GetNavigationController();
            var site = new Site();
            A.CallTo(() => _siteService.GetCurrentSite()).Returns(site);
            A.CallTo(() => navigationService.GetLayoutList()).Returns(new SiteTree<Layout>());

            PartialViewResult partialViewResult = navigationController.LayoutTree();

            partialViewResult.Model.Should().BeOfType<SiteTree<Layout>>();
        }

        [Fact]
        public void NavigationController_MediaTree_ShouldReturnSiteTreeAsModel()
        {
            NavigationController navigationController = GetNavigationController();
            A.CallTo(() => navigationService.GetMediaTree()).Returns(new SiteTree<MediaCategory>());

            PartialViewResult partialViewResult = navigationController.MediaTree();

            partialViewResult.Model.Should().BeOfType<SiteTree<MediaCategory>>();
        }

        [Fact]
        public void NavigationController_UserList_ShouldCallUserList()
        {
            NavigationController navigationController = GetNavigationController();

            navigationController.UserList();

            A.CallTo(() => navigationService.GetUserList()).MustHaveHappened();
        }

        [Fact]
        public void NavigationController_UserList_ShouldReturnAPartialViewResult()
        {
            NavigationController navigationController = GetNavigationController();

            PartialViewResult result = navigationController.UserList();

            result.Should().NotBeNull();
        }

        [Fact]
        public void NavigationController_UserList_ModelShouldBeResponseOfNavigationService()
        {
            NavigationController navigationController = GetNavigationController();
            var siteTree = new SiteTree<User>();
            A.CallTo(() => navigationService.GetUserList()).Returns(siteTree);

            PartialViewResult result = navigationController.UserList();

            result.Model.Should().Be(siteTree);
        }

        [Fact]
        public void NavigationController_Navlinks_ShouldReturnAPartialViewResult()
        {
            NavigationController navigationController = GetNavigationController();

            navigationController.NavLinks().Should().BeOfType<PartialViewResult>();
        }
    }
}