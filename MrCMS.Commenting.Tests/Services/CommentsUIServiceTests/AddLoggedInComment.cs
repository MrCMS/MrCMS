using FluentAssertions;
using MrCMS.Commenting.Tests.Stubs;
using MrCMS.Commenting.Tests.Support;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Commenting.Entities;
using MrCMS.Web.Apps.Commenting.Models;
using MrCMS.Web.Apps.Commenting.Services;
using MrCMS.Web.Apps.Commenting.Settings;
using MrCMS.Website;
using Xunit;
using Xunit.Extensions;

namespace MrCMS.Commenting.Tests.Services.CommentsUIServiceTests
{
    public class AddLoggedInComment : InMemoryDatabaseTest
    {
        [Fact]
        public void AddsAComment()
        {
            var basicMappedWebpage = GetBasicMappedWebpage();
            LoggedInUserAddCommentModel model = new LoggedInAddCommentModelBuilder().ForWebpage(basicMappedWebpage).Build();
            CommentsUIService service = new CommentsUIServiceBuilder().WithSession(Session).Build();
            //ensuring there were no existing comments
            Session.QueryOver<Comment>().RowCount().Should().Be(0);

            PostCommentResponse response = service.AddLoggedInComment(model);

            Session.QueryOver<Comment>().RowCount().Should().Be(1);
        }

        private BasicMappedWebpage GetBasicMappedWebpage()
        {
            var basicMappedWebpage = new CommentingEnabledWebpageBuilder().Build();
            Session.Transact(session => session.Save(basicMappedWebpage));
            return basicMappedWebpage;
        }

        [Fact]
        public void SetsTheCurrentUserToBeTheCommentUser()
        {
            var basicMappedWebpage = GetBasicMappedWebpage();
            LoggedInUserAddCommentModel model = new LoggedInAddCommentModelBuilder().ForWebpage(basicMappedWebpage).Build();
            CommentsUIService service = new CommentsUIServiceBuilder().WithSession(Session).Build();
            //ensuring there were no existing comments
            Session.QueryOver<Comment>().RowCount().Should().Be(0);

            PostCommentResponse response = service.AddLoggedInComment(model);

            Comment comment = Session.QueryOver<Comment>().List()[0];
            comment.User.Should().NotBeNull();
            comment.User.Should().Be(CurrentRequestData.CurrentUser);
        }

        [Fact]
        public void IfTheCurrentUserIsNotSetDoNotAddAComment()
        {
            var basicMappedWebpage = GetBasicMappedWebpage();
            LoggedInUserAddCommentModel model = new LoggedInAddCommentModelBuilder().ForWebpage(basicMappedWebpage).Build();
            CommentsUIService service = new CommentsUIServiceBuilder().WithSession(Session).Build();
            CurrentRequestData.CurrentUser = null;
            //ensuring there were no existing comments
            Session.QueryOver<Comment>().RowCount().Should().Be(0);

            PostCommentResponse response = service.AddLoggedInComment(model);

            Session.QueryOver<Comment>().RowCount().Should().Be(0);
        }

        [Fact]
        public void IfTheCurrentUserIsNotSetTheResultShouldBeInvalid()
        {
            var basicMappedWebpage = GetBasicMappedWebpage();
            LoggedInUserAddCommentModel model = new LoggedInAddCommentModelBuilder().ForWebpage(basicMappedWebpage).Build();
            CommentsUIService service = new CommentsUIServiceBuilder().WithSession(Session).Build();
            CurrentRequestData.CurrentUser = null;

            PostCommentResponse response = service.AddLoggedInComment(model);

            response.Valid.Should().BeFalse();
        }

        [Fact]
        public void ShouldSetNameAndEmailFromCurrentUser()
        {
            var basicMappedWebpage = GetBasicMappedWebpage();
            LoggedInUserAddCommentModel model = new LoggedInAddCommentModelBuilder().ForWebpage(basicMappedWebpage).Build();
            CommentsUIService service = new CommentsUIServiceBuilder().WithSession(Session).Build();
            CurrentRequestData.CurrentUser.FirstName = "test";
            CurrentRequestData.CurrentUser.LastName = "name";
            CurrentRequestData.CurrentUser.Email = "test@example.com";

            PostCommentResponse response = service.AddLoggedInComment(model);

            Comment comment = Session.QueryOver<Comment>().SingleOrDefault();
            comment.Name.Should().Be("test name");
            comment.Email.Should().Be("test@example.com");
        }

        [Fact]
        public void ShouldSetTheWebpage()
        {
            BasicMappedWebpage webpage = new CommentingMappedWebpageBuilder().Build();
            Session.Transact(session => session.Save(webpage));
            LoggedInUserAddCommentModel model = new LoggedInAddCommentModelBuilder().ForWebpage(webpage).Build();
            CommentsUIService service = new CommentsUIServiceBuilder().WithSession(Session).Build();

            PostCommentResponse response = service.AddLoggedInComment(model);

            Comment comment = Session.QueryOver<Comment>().SingleOrDefault();
            comment.Webpage.Should().Be(webpage);
        }

        [Fact]
        public void ShouldSetTheMessage()
        {
            var basicMappedWebpage = GetBasicMappedWebpage();
            LoggedInUserAddCommentModel model = new LoggedInAddCommentModelBuilder().ForWebpage(basicMappedWebpage).WithMessage("test message").Build();
            CommentsUIService service = new CommentsUIServiceBuilder().WithSession(Session).Build();

            PostCommentResponse response = service.AddLoggedInComment(model);

            Comment comment = Session.QueryOver<Comment>().SingleOrDefault();
            comment.Message.Should().Be("test message");
        }

        [Theory]
        [InlineData(CommentApprovalType.None, true)]
        [InlineData(CommentApprovalType.Guest, true)]
        [InlineData(CommentApprovalType.All, null)]
        public void ShouldSetApprovalTypeBasedOnSetting(CommentApprovalType approvalType, bool? approved)
        {
            var basicMappedWebpage = GetBasicMappedWebpage();
            LoggedInUserAddCommentModel model = new LoggedInAddCommentModelBuilder().ForWebpage(basicMappedWebpage).WithMessage("test message").Build();
            CommentsUIService service =
                new CommentsUIServiceBuilder().WithApprovalType(approvalType).WithSession(Session).Build();

            PostCommentResponse response = service.AddLoggedInComment(model);

            Comment comment = Session.QueryOver<Comment>().SingleOrDefault();
            comment.Approved.Should().Be(approved);
        }

        [Theory]
        [InlineData(CommentApprovalType.None, false)]
        [InlineData(CommentApprovalType.Guest, false)]
        [InlineData(CommentApprovalType.All, true)]
        public void ResponsePendingShouldBeBasedOnApprovalType(CommentApprovalType approvalType, bool pending)
        {
            var basicMappedWebpage = GetBasicMappedWebpage();
            LoggedInUserAddCommentModel model = new LoggedInAddCommentModelBuilder().ForWebpage(basicMappedWebpage).WithMessage("test message").Build();
            CommentsUIService service =
                new CommentsUIServiceBuilder().WithApprovalType(approvalType).WithSession(Session).Build();

            PostCommentResponse response = service.AddLoggedInComment(model);

            response.Pending.Should().Be(pending);
        }

        [Fact]
        public void IfCommentIsApprovedReturnApprovedMessage()
        {
            var basicMappedWebpage = GetBasicMappedWebpage();
            LoggedInUserAddCommentModel model = new LoggedInAddCommentModelBuilder().ForWebpage(basicMappedWebpage).Build();
            CommentsUIService service = new CommentsUIServiceBuilder().WithGuestCommentsEnabled()
                                                                      .WithApprovalType(CommentApprovalType.None)
                                                                      .WithApprovedMessage("comment approved")
                                                                      .WithSession(Session)
                                                                      .Build();

            PostCommentResponse response = service.AddLoggedInComment(model);

            response.Message.Should().Be("comment approved");
        }

        [Fact]
        public void IfCommentIsPendingReturnPendingMessage()
        {
            var basicMappedWebpage = GetBasicMappedWebpage();
            LoggedInUserAddCommentModel model = new LoggedInAddCommentModelBuilder().ForWebpage(basicMappedWebpage).Build();
            CommentsUIService service = new CommentsUIServiceBuilder().WithGuestCommentsEnabled()
                                                                      .WithApprovalType(CommentApprovalType.All)
                                                                      .WithPendingMessage("comment pending")
                                                                      .WithSession(Session)
                                                                      .Build();

            PostCommentResponse response = service.AddLoggedInComment(model);

            response.Message.Should().Be("comment pending");
        }
    }
}