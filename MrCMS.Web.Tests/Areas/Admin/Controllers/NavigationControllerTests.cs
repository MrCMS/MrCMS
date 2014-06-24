using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Controllers;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Web.Areas.Admin.Services;
using Xunit;

namespace MrCMS.Web.Tests.Areas.Admin.Controllers
{
    public class NavigationControllerTests
    {
        private readonly NavigationController _navigationController;
        private readonly ITreeNavService _treeNavService;

        public NavigationControllerTests()
        {
            _treeNavService = A.Fake<ITreeNavService>();
            _navigationController = new NavigationController(_treeNavService);
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
            A.CallTo(() => _treeNavService.GetWebpageNodes(null)).Returns(new AdminTree());

            PartialViewResult partialViewResult = _navigationController.WebSiteTree(null);

            partialViewResult.Model.Should().BeOfType<AdminTree>();
        }

        [Fact]
        public void NavigationController_LayoutTree_ShouldReturnLayoutTreeAsModel()
        {
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
    }
}