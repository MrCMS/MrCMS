using FluentAssertions;
using MrCMS.Commenting.Tests.Stubs;
using MrCMS.Commenting.Tests.Support;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Commenting.Entities;
using MrCMS.Web.Apps.Commenting.Models;
using MrCMS.Web.Apps.Commenting.Services;
using MrCMS.Web.Apps.Commenting.Settings;
using Xunit;
using Xunit.Extensions;

namespace MrCMS.Commenting.Tests.Services.CommentsUIServiceTests
{
    public class AddGuestComment : InMemoryDatabaseTest
    {
        [Fact]
        public void WithGuestCommentsEnabledAddsAComment()
        {
            var webpage = new CommentingEnabledWebpageBuilder().Build();
            Session.Transact(session => session.Save(webpage));
            GuestAddCommentModel model = new GuestAddCommentModelBuilder().ForWebpage(webpage).Build();
            CommentsUIService service =
                new CommentsUIServiceBuilder().WithGuestCommentsEnabled().WithSession(Session).Build();
            // confirming none have been added
            Session.QueryOver<Comment>().RowCount().Should().Be(0);

            service.AddGuestComment(model);

            Session.QueryOver<Comment>().RowCount().Should().Be(1);
        }

        [Fact]
        public void WithGuestCommentsEnabledReturnsAValidResponse()
        {
            var webpage = new CommentingEnabledWebpageBuilder().Build();
            Session.Transact(session => session.Save(webpage));
            GuestAddCommentModel model = new GuestAddCommentModelBuilder().ForWebpage(webpage).Build();
            CommentsUIService service =
                new CommentsUIServiceBuilder().WithGuestCommentsEnabled().WithSession(Session).Build();

            PostCommentResponse response = service.AddGuestComment(model);

            response.Valid.Should().BeTrue();
        }

        [Fact]
        public void AddedCommentShouldHavePropertiesSet()
        {
            BasicMappedWebpage webpage = new CommentingMappedWebpageBuilder().Build();
            Session.Transact(session => session.Save(webpage));
            GuestAddCommentModel model = new GuestAddCommentModelBuilder().WithName("test name")
                                                                          .WithEmail("test@example.com")
                                                                          .WithMessage("test message")
                                                                          .ForWebpage(webpage)
                                                                          .Build();
            CommentsUIService service = new CommentsUIServiceBuilder().WithGuestCommentsEnabled()
                                                                      .WithSession(Session)
                                                                      .Build();

            service.AddGuestComment(model);

            Comment comment = Session.QueryOver<Comment>().SingleOrDefault();
            comment.Name.Should().Be("test name");
            comment.Email.Should().Be("test@example.com");
            comment.Message.Should().Be("test message");
            comment.Webpage.Should().Be(webpage);
        }

        [Fact]
        public void WithGuestCommentsDisabledDoesNotAddAComment()
        {
            GuestAddCommentModel model = new GuestAddCommentModelBuilder().Build();
            CommentsUIService service =
                new CommentsUIServiceBuilder().WithGuestCommentsDisabled().WithSession(Session).Build();

            service.AddGuestComment(model);

            Session.QueryOver<Comment>().RowCount().Should().Be(0);
        }

        [Fact]
        public void WithGuestCommentsDisabledReturnsInvalid()
        {
            GuestAddCommentModel model = new GuestAddCommentModelBuilder().Build();
            CommentsUIService service =
                new CommentsUIServiceBuilder().WithGuestCommentsDisabled().WithSession(Session).Build();

            PostCommentResponse response = service.AddGuestComment(model);

            response.Valid.Should().BeFalse();
        }

        [Theory]
        [InlineData(CommentApprovalType.None, true)]
        [InlineData(CommentApprovalType.Guest, null)]
        [InlineData(CommentApprovalType.All, null)]
        public void ShouldSetApprovalTypeBasedOnSetting(CommentApprovalType approvalType, bool? approved)
        {
            var webpage = new CommentingEnabledWebpageBuilder().Build();
            Session.Transact(session => session.Save(webpage));
            GuestAddCommentModel model = new GuestAddCommentModelBuilder().ForWebpage(webpage).Build();
            CommentsUIService service = new CommentsUIServiceBuilder().WithGuestCommentsEnabled()
                                                                      .WithApprovalType(approvalType)
                                                                      .WithSession(Session)
                                                                      .Build();

            service.AddGuestComment(model);

            Comment comment = Session.QueryOver<Comment>().SingleOrDefault();
            comment.Approved.Should().Be(approved);
        }

        [Theory]
        [InlineData(CommentApprovalType.None, false)]
        [InlineData(CommentApprovalType.Guest, true)]
        [InlineData(CommentApprovalType.All, true)]
        public void ResponsePendingShouldBeBasedOnApprovalType(CommentApprovalType approvalType, bool pending)
        {
            var webpage = new CommentingEnabledWebpageBuilder().Build();
            Session.Transact(session => session.Save(webpage));
            GuestAddCommentModel model = new GuestAddCommentModelBuilder().ForWebpage(webpage).Build();
            CommentsUIService service = new CommentsUIServiceBuilder().WithGuestCommentsEnabled()
                                                                      .WithApprovalType(approvalType)
                                                                      .WithSession(Session)
                                                                      .Build();

            var response = service.AddGuestComment(model);

            response.Pending.Should().Be(pending);
        }

        [Fact]
        public void IfCommentIsApprovedReturnApprovedMessage()
        {
            var webpage = new CommentingEnabledWebpageBuilder().Build();
            Session.Transact(session => session.Save(webpage));
            GuestAddCommentModel model = new GuestAddCommentModelBuilder().ForWebpage(webpage).Build();
            CommentsUIService service = new CommentsUIServiceBuilder().WithGuestCommentsEnabled()
                                                                      .WithApprovalType(CommentApprovalType.None)
                                                                      .WithApprovedMessage("comment approved")
                                                                      .WithSession(Session)
                                                                      .Build();

            PostCommentResponse response = service.AddGuestComment(model);

            response.Message.Should().Be("comment approved");
        }

        [Fact]
        public void IfCommentIsPendingReturnPendingMessage()
        {
            var webpage = new CommentingEnabledWebpageBuilder().Build();
            Session.Transact(session => session.Save(webpage));
            GuestAddCommentModel model = new GuestAddCommentModelBuilder().ForWebpage(webpage).Build();
            CommentsUIService service = new CommentsUIServiceBuilder().WithGuestCommentsEnabled()
                                                                      .WithApprovalType(CommentApprovalType.Guest)
                                                                      .WithPendingMessage("comment pending")
                                                                      .WithSession(Session)
                                                                      .Build();

            PostCommentResponse response = service.AddGuestComment(model);

            response.Message.Should().Be("comment pending");
        }
    }
}