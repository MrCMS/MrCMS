using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Controllers;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Web.Tests.Stubs;
using Xunit;

namespace MrCMS.Web.Tests.Areas.Admin.Controllers
{
    public class TagControllerTests
    {
        [Fact]
        public void TagController_Search_ShouldCallTagServiceSearch()
        {
            var tagService = A.Fake<ITagAdminService>();
            var tagController = new TagController(tagService);

            tagController.Search( "test");

            A.CallTo(() => tagService.Search("test")).MustHaveHappened();
        }

        [Fact]
        public void TagController_Search_ShouldReturnTheResultOfTheServiceQuery()
        {
            var tagService = A.Fake<ITagAdminService>();
            var tagController = new TagController(tagService);
            IEnumerable<AutoCompleteResult> results = Enumerable.Empty<AutoCompleteResult>();
            A.CallTo(() => tagService.Search("test")).Returns(results);

            JsonResult result = tagController.Search("test");
            result.Data.As<IEnumerable<AutoCompleteResult>>().Should().BeEquivalentTo(results);
        }
    }
}