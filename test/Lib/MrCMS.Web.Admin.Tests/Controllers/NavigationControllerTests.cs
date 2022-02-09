using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Admin.Controllers;
using MrCMS.Web.Admin.Models;
using MrCMS.Web.Admin.Services;
using Xunit;

namespace MrCMS.Web.Admin.Tests.Controllers
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
        public async Task NavigationController_WebsiteTree_ShouldReturnPartialView()
        {
            PartialViewResult partialViewResult = await _navigationController.WebSiteTree(null);

            partialViewResult.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public async Task NavigationController_WebsiteTree_ShouldReturnWebsiteTreeAsModel()
        {
            A.CallTo(() => _treeNavService.GetWebpageNodes(null)).Returns(new AdminTree());

            PartialViewResult partialViewResult = await _navigationController.WebSiteTree(null);

            partialViewResult.Model.Should().BeOfType<AdminTree>();
        }

        [Fact]
        public async Task NavigationController_LayoutTree_ShouldReturnLayoutTreeAsModel()
        {
            A.CallTo(() => _treeNavService.GetLayoutNodes(null)).Returns(new AdminTree());

            PartialViewResult partialViewResult = await _navigationController.LayoutTree(null);

            partialViewResult.Model.Should().BeOfType<AdminTree>();
        }

        [Fact]
        public async Task NavigationController_MediaTree_ShouldReturnSiteTreeAsModel()
        {
            A.CallTo(() => _treeNavService.GetMediaCategoryNodes(null)).Returns(new AdminTree());

            PartialViewResult partialViewResult = await _navigationController.MediaTree(null);

            partialViewResult.Model.Should().BeOfType<AdminTree>();
        }
    }
}