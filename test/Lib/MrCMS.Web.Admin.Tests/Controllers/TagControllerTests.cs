using System.Collections.Generic;
using System.Linq;
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
            result.Value.As<IEnumerable<AutoCompleteResult>>().Should().BeEquivalentTo(results);
        }
    }
}