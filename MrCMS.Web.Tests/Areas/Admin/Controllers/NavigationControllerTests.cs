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
        private INavigationService _navigationService;
        private ICurrentSiteLocator _currentSiteLocator;
        private NavigationController _navigationController;

        public NavigationControllerTests()
        {

            _navigationService = A.Fake<INavigationService>();
            _siteService = A.Fake<ISiteService>();
            _currentSiteLocator = A.Fake<ICurrentSiteLocator>();
            _navigationController = new NavigationController(_navigationService, _siteService, _currentSiteLocator);
        }

        [Fact]
        public void NavigationController_WebsiteTree_ShouldReturnPartialView()
        {
            PartialViewResult partialViewResult = _navigationController.WebSiteTree();

            partialViewResult.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void NavigationController_WebsiteTree_ShouldReturnWebsiteTreeAsModel()
        {
            var site = new Site();
            A.CallTo(() => _currentSiteLocator.GetCurrentSite()).Returns(site);
            A.CallTo(() => _navigationService.GetWebsiteTree(null)).Returns(new SiteTree<Webpage>());

            PartialViewResult partialViewResult = _navigationController.WebSiteTree();

            partialViewResult.Model.Should().BeOfType<SiteTree<Webpage>>();
        }

        [Fact]
        public void NavigationController_LayoutTree_ShouldReturnLayoutTreeAsModel()
        {
            var site = new Site();
            A.CallTo(() => _currentSiteLocator.GetCurrentSite()).Returns(site);
            A.CallTo(() => _navigationService.GetLayoutList()).Returns(new SiteTree<Layout>());

            PartialViewResult partialViewResult = _navigationController.LayoutTree();

            partialViewResult.Model.Should().BeOfType<SiteTree<Layout>>();
        }

        [Fact]
        public void NavigationController_MediaTree_ShouldReturnSiteTreeAsModel()
        {
            A.CallTo(() => _navigationService.GetMediaTree()).Returns(new SiteTree<MediaCategory>());

            PartialViewResult partialViewResult = _navigationController.MediaTree();

            partialViewResult.Model.Should().BeOfType<SiteTree<MediaCategory>>();
        }

        [Fact]
        public void NavigationController_Navlinks_ShouldReturnAPartialViewResult()
        {
            _navigationController.NavLinks().Should().BeOfType<PartialViewResult>();
        }
    }
}