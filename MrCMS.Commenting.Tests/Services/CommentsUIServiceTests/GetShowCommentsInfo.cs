using System.Collections.Generic;
using MrCMS.Commenting.Tests.Stubs;
using MrCMS.Entities.People;
using MrCMS.Web.Apps.Commenting.Entities;
using MrCMS.Web.Apps.Commenting.Models;
using MrCMS.Web.Apps.Commenting.Services;
using MrCMS.Website;
using Xunit;
using MrCMS.Commenting.Tests.Support;
using FluentAssertions;
using MrCMS.Helpers;
using System.Linq;

namespace MrCMS.Commenting.Tests.Services.CommentsUIServiceTests
{
    public class GetShowCommentsInfo : InMemoryDatabaseTest
    {
        [Fact]
        public void IfCommentingInWidgetIsDisabledDisabledIsTrue()
        {
            BasicMappedWebpage webpage =
                new CommentingMappedWebpageBuilder().WithCommentsArea()
                                                    .WithCommentsWidget()
                                                    .WithCommentingDisabled()
                                                    .Build();
            CommentsUIService commentsUIService = new CommentsUIServiceBuilder().Build();

            CommentsViewInfo info = commentsUIService.GetShowCommentsInfo(webpage);

            info.Disabled.Should().BeTrue();
        }

        [Fact]
        public void IfCommentingInWidgetIsEnabledReturnViewShow()
        {
            BasicMappedWebpage webpage = new CommentingEnabledWebpageBuilder().Build();
            CommentsUIService commentsUIService = new CommentsUIServiceBuilder().Build();

            CommentsViewInfo info = commentsUIService.GetShowCommentsInfo(webpage);

            info.View.Should().Be("Show");
        }

        [Fact]
        public void IfCommentingInWidgetIsEnabledReturnModelOfAssociatedComments()
        {
            BasicMappedWebpage webpage = new CommentingEnabledWebpageBuilder().Build();
            Session.Transact(session => session.Save(webpage));
            List<Comment> comments = new List<Comment>();
            10.Times(() => Session.SaveAndAddEntityTo(new CommentBuilder().IsApproved().ForWebpage(webpage).Build(), comments));
            var service = new CommentsUIServiceBuilder().WithSession(Session).Build();

            CommentsViewInfo info = service.GetShowCommentsInfo(webpage);

            info.Model.As<List<Comment>>().Should().BeEquivalentTo(comments);
        }

        [Fact]
        public void ShouldOnlyReturnApprovedComments()
        {
            BasicMappedWebpage webpage = new CommentingEnabledWebpageBuilder().Build();
            Session.Transact(session => session.Save(webpage));
            List<Comment> comments = new List<Comment>();
            10.Times(() => Session.SaveAndAddEntityTo(new CommentBuilder().IsNotApproved().ForWebpage(webpage).Build(), comments));
            10.Times(() => Session.SaveAndAddEntityTo(new CommentBuilder().IsApproved().ForWebpage(webpage).Build(), comments));
            var service = new CommentsUIServiceBuilder().WithSession(Session).Build();

            CommentsViewInfo info = service.GetShowCommentsInfo(webpage);

            info.Model.As<List<Comment>>().Should().BeEquivalentTo(comments.Skip(10));
        }

        [Fact(Skip="removing progressive loading for initial implementation")]
        public void IfLimitIsSetShouldOnlyReturnThatManyComments()
        {
            BasicMappedWebpage webpage = new CommentingEnabledWebpageBuilder().Build();
            Session.Transact(session => session.Save(webpage));
            List<Comment> comments = new List<Comment>();
            10.Times(() => Session.SaveAndAddEntityTo(new CommentBuilder().IsApproved().ForWebpage(webpage).Build(), comments));
            var service = new CommentsUIServiceBuilder().WithSession(Session).WithCommentLimit(5).Build();

            CommentsViewInfo info = service.GetShowCommentsInfo(webpage);

            info.Model.As<List<Comment>>().Should().BeEquivalentTo(comments.Take(5));
        }

        [Fact]
        public void IfGuestPostsAreDisabledAndIsGuestViewDataAllowReplyShouldBeFalse()
        {
            BasicMappedWebpage webpage = new CommentingEnabledWebpageBuilder().Build();
            Session.Transact(session => session.Save(webpage));
            CurrentRequestData.CurrentUser = null;
            var service = new CommentsUIServiceBuilder().WithSession(Session).WithGuestCommentsDisabled().Build();

            CommentsViewInfo info = service.GetShowCommentsInfo(webpage);

            info.ViewData["allow-reply"].Should().Be(false);
        }

        [Fact]
        public void IfGuestPostsAreEnabledAndIsGuestViewDataAllowReplyShouldBeFalse()
        {
            BasicMappedWebpage webpage = new CommentingEnabledWebpageBuilder().Build();
            Session.Transact(session => session.Save(webpage));
            CurrentRequestData.CurrentUser = null;
            var service = new CommentsUIServiceBuilder().WithSession(Session).WithGuestCommentsEnabled().Build();

            CommentsViewInfo info = service.GetShowCommentsInfo(webpage);

            info.ViewData["allow-reply"].Should().Be(true);
        }

        [Fact]
        public void IfLoggedInViewDataAllowReplyShouldBeTrue()
        {
            BasicMappedWebpage webpage = new CommentingEnabledWebpageBuilder().Build();
            Session.Transact(session => session.Save(webpage));
            CurrentRequestData.CurrentUser = new User();
            var service = new CommentsUIServiceBuilder().WithSession(Session).WithGuestCommentsDisabled().Build();

            CommentsViewInfo info = service.GetShowCommentsInfo(webpage);

            info.ViewData["allow-reply"].Should().Be(true);
        }

        [Fact]
        public void ViewDataWebpageShouldBePassedWebpage()
        {
            BasicMappedWebpage webpage = new CommentingEnabledWebpageBuilder().Build();
            Session.Transact(session => session.Save(webpage));
            var service = new CommentsUIServiceBuilder().WithSession(Session).WithGuestCommentsDisabled().Build();

            CommentsViewInfo info = service.GetShowCommentsInfo(webpage);

            info.ViewData["webpage"].Should().Be(webpage);
        }


        [Fact]
        public void ShouldOnlyReturnRootComments()
        {
            BasicMappedWebpage webpage = new CommentingEnabledWebpageBuilder().Build();
            Session.Transact(session => session.Save(webpage));
            List<Comment> comments = new List<Comment>();
            5.Times(() => Session.SaveAndAddEntityTo(new CommentBuilder().IsApproved().ForWebpage(webpage).Build(), comments));
            5.Times(() => Session.SaveAndAddEntityTo(new CommentBuilder().IsApproved().InReplyTo(comments[0]).ForWebpage(webpage).Build(), comments));
            var service = new CommentsUIServiceBuilder().WithSession(Session).Build();

            CommentsViewInfo info = service.GetShowCommentsInfo(webpage);

            info.Model.As<List<Comment>>().Should().BeEquivalentTo(comments.Take(5));
        }
    }
}