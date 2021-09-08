using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Models;
using MrCMS.Web.Admin.Controllers;
using MrCMS.Web.Admin.Services;
using Xunit;

namespace MrCMS.Web.Admin.Tests.Controllers
{
    public class TagControllerTests
    {
        [Fact]
        public async Task TagController_Search_ShouldCallTagServiceSearch()
        {
            var tagService = A.Fake<ITagAdminService>();
            var tagController = new TagController(tagService);

            await tagController.Search( "test");

            A.CallTo(() => tagService.Search("test")).MustHaveHappened();
        }

        [Fact]
        public async Task TagController_Search_ShouldReturnTheResultOfTheServiceQuery()
        {
            var tagService = A.Fake<ITagAdminService>();
            var tagController = new TagController(tagService);
            IEnumerable<AutoCompleteResult> results = Enumerable.Empty<AutoCompleteResult>();
            A.CallTo(() => tagService.Search("test")).Returns(results);

            JsonResult result = await tagController.Search("test");
            result.Value.As<IEnumerable<AutoCompleteResult>>().Should().BeEquivalentTo(results);
        }
    }
}