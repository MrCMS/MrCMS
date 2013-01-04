using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Controllers;
using Xunit;

namespace MrCMS.Web.Tests.Areas.Admin.Controllers
{
    public class TagControllerTests
    {
        [Fact]
        public void TagController_Search_ShouldCallTagServiceSearch()
        {
            var tagService = A.Fake<ITagService>();
            var tagController = new TagController(tagService);

            tagController.Search("test", 1);

            A.CallTo(() => tagService.Search("test", 1)).MustHaveHappened();
        }

        [Fact]
        public void TagController_Search_ShouldReturnTheResultOfTheServiceQuery()
        {
            var tagService = A.Fake<ITagService>();
            var tagController = new TagController(tagService);
            IEnumerable<AutoCompleteResult> results = Enumerable.Empty<AutoCompleteResult>();
            A.CallTo(() => tagService.Search("test", 1)).Returns(results);

            JsonResult result = tagController.Search("test", 1);
            result.Data.As<IEnumerable<AutoCompleteResult>>().Should().BeEquivalentTo(results);
        }
    }
}