using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Commenting.Models;
using MrCMS.Web.Apps.Commenting.Services;
using Xunit;

namespace MrCMS.Commenting.Tests.Controllers.CommentsControllerTests
{
    public class Add
    {
        [Fact]
        public void ReturnsAPartialViewResult()
        {
            var controller = new CommentsControllerBuilder().Build();

            var result = controller.Add(A<Webpage>._).As<PartialViewResult>();

            result.Should().NotBeNull();
        }

        [Fact]
        public void ReturnsAPartialViewWithTheViewNameFromTheServiceCall()
        {
            var webpage = A<Webpage>._;
            var commentsUIService = A.Fake<ICommentsUIService>();
            A.CallTo(() => commentsUIService.GetAddCommentsInfo(webpage))
             .Returns(new CommentsViewInfo { View = "test-view" });
            var controller = new CommentsControllerBuilder().WithService(commentsUIService).Build();

            var result = controller.Add(webpage).As<PartialViewResult>();

            result.ViewName.Should().Be("test-view");
        }

        [Fact]
        public void ReturnsAPartialViewWithTheModelFromTheServiceCall()
        {
            var webpage = A<Webpage>._;
            var commentsUIService = A.Fake<ICommentsUIService>();
            var model = new object();
            A.CallTo(() => commentsUIService.GetAddCommentsInfo(webpage))
             .Returns(new CommentsViewInfo { Model = model });
            var controller = new CommentsControllerBuilder().WithService(commentsUIService).Build();

            var result = controller.Add(webpage).As<PartialViewResult>();

            result.Model.Should().Be(model);
        }

        [Fact]
        public void ReturnsAPartialViewWithTheViewDataFromTheServiceCall()
        {
            var webpage = A<Webpage>._;
            var commentsUIService = A.Fake<ICommentsUIService>();
            var viewDataDictionary = new ViewDataDictionary { { "test", new object() } };
            A.CallTo(() => commentsUIService.GetAddCommentsInfo(webpage))
             .Returns(new CommentsViewInfo { ViewData = viewDataDictionary });
            var controller = new CommentsControllerBuilder().WithService(commentsUIService).Build();

            var result = controller.Add(webpage).As<PartialViewResult>();

            result.ViewData.Should().Equal(viewDataDictionary);
        }

        [Fact]
        public void IfResultOfCallHasDisabledTrueReturnAnEmptyResult()
        {
            var webpage = A<Webpage>._;
            var commentsUIService = A.Fake<ICommentsUIService>();
            A.CallTo(() => commentsUIService.GetAddCommentsInfo(webpage)).Returns(new CommentsViewInfo { Disabled = true });
            var controller = new CommentsControllerBuilder().WithService(commentsUIService).Build();

            var result = controller.Add(webpage);

            result.Should().BeOfType<EmptyResult>();
        }
    }
}