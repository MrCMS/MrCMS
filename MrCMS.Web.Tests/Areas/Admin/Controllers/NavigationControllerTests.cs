using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Multisite;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Controllers;
using Xunit;

namespace MrCMS.Web.Tests.Areas.Admin.Controllers
{
    public class NavigationControllerTests
    {
        private readonly ICurrentSiteLocator _currentSiteLocator;
        private readonly NavigationController _navigationController;
        private readonly INavigationService _navigationService;
        private readonly ISiteService _siteService;
        private readonly ITreeNavService _treeNavService;

        public NavigationControllerTests()
        {
            _navigationService = A.Fake<INavigationService>();
            _siteService = A.Fake<ISiteService>();
            _currentSiteLocator = A.Fake<ICurrentSiteLocator>();
            _treeNavService = A.Fake<ITreeNavService>();
            _navigationController = new NavigationController(_navigationService, _treeNavService, _siteService,
                                                             _currentSiteLocator);
        }

        [Fact]
        public void NavigationController_WebsiteTree_ShouldReturnPartialView()
        {
            PartialViewResult partialViewResult = _navigationController.WebSiteTree(null);

            partialViewResult.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void NavigationController_WebsiteTree_ShouldReturnWebsiteTreeAsModel()
        {
            var site = new Site();
            A.CallTo(() => _currentSiteLocator.GetCurrentSite()).Returns(site);
            A.CallTo(() => _treeNavService.GetWebpageNodes(null)).Returns(new AdminTree());

            PartialViewResult partialViewResult = _navigationController.WebSiteTree(null);

            partialViewResult.Model.Should().BeOfType<AdminTree>();
        }

        [Fact]
        public void NavigationController_LayoutTree_ShouldReturnLayoutTreeAsModel()
        {
            var site = new Site();
            A.CallTo(() => _currentSiteLocator.GetCurrentSite()).Returns(site);
            A.CallTo(() => _treeNavService.GetLayoutNodes(null)).Returns(new AdminTree());

            PartialViewResult partialViewResult = _navigationController.LayoutTree(null);

            partialViewResult.Model.Should().BeOfType<AdminTree>();
        }

        [Fact]
        public void NavigationController_MediaTree_ShouldReturnSiteTreeAsModel()
        {
            A.CallTo(() => _treeNavService.GetMediaCategoryNodes(null)).Returns(new AdminTree());

            PartialViewResult partialViewResult = _navigationController.MediaTree(null);

            partialViewResult.Model.Should().BeOfType<AdminTree>();
        }

        [Fact]
        public void NavigationController_Navlinks_ShouldReturnAPartialViewResult()
        {
            _navigationController.NavLinks().Should().BeOfType<PartialViewResult>();
        }
    }
}