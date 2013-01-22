using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Controllers;
using MrCMS.Web.Tests.Stubs;
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

            var stubDocument = new StubDocument();
            tagController.Search(stubDocument, "test");

            A.CallTo(() => tagService.Search(stubDocument, "test")).MustHaveHappened();
        }

        [Fact]
        public void TagController_Search_ShouldReturnTheResultOfTheServiceQuery()
        {
            var tagService = A.Fake<ITagService>();
            var tagController = new TagController(tagService);
            IEnumerable<AutoCompleteResult> results = Enumerable.Empty<AutoCompleteResult>();
            var stubDocument = new StubDocument();
            A.CallTo(() => tagService.Search(stubDocument, "test")).Returns(results);

            JsonResult result = tagController.Search(stubDocument, "test");
            result.Data.As<IEnumerable<AutoCompleteResult>>().Should().BeEquivalentTo(results);
        }
    }
}