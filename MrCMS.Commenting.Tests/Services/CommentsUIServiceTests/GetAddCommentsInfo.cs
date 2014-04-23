using FluentAssertions;
using MrCMS.Commenting.Tests.Stubs;
using MrCMS.Commenting.Tests.Support;
using MrCMS.Entities.People;
using MrCMS.Web.Apps.Commenting.Models;
using MrCMS.Web.Apps.Commenting.Services;
using MrCMS.Web.Apps.Core.Models;
using MrCMS.Web.Apps.Core.Models.RegisterAndLogin;
using MrCMS.Website;
using Xunit;

namespace MrCMS.Commenting.Tests.Services.CommentsUIServiceTests
{
    public class GetAddCommentsInfo : MrCMSTest
    {
        [Fact]
        public void IfCommentingInWidgetIsDisabledDisabledIsTrue()
        {
            BasicMappedWebpage basicMappedWebpage =
                new CommentingMappedWebpageBuilder().WithCommentsArea()
                                                    .WithCommentsWidget()
                                                    .WithCommentingDisabled()
                                                    .Build();
            CommentsUIService commentsUIService = new CommentsUIServiceBuilder().Build();

            CommentsViewInfo info = commentsUIService.GetAddCommentsInfo(basicMappedWebpage);

            info.Disabled.Should().BeTrue();
        }

        [Fact]
        public void IfCommentingInWidgetNotDisabledDisabledIsFalse()
        {
            BasicMappedWebpage basicMappedWebpage =
                new CommentingMappedWebpageBuilder().WithCommentsArea()
                                                    .WithCommentsWidget()
                                                    .Build();
            CommentsUIService commentsUIService = new CommentsUIServiceBuilder().Build();

            CommentsViewInfo info = commentsUIService.GetAddCommentsInfo(basicMappedWebpage);

            info.Disabled.Should().BeFalse();
        }

        [Fact]
        public void IfGuestCommentsAreTurnedOffAndCurrentUserIsNullViewNameShouldBeLogin()
        {
            CurrentRequestData.CurrentUser = null;
            CommentsUIService commentsUIService = new CommentsUIServiceBuilder().WithGuestCommentsDisabled().Build();

            var info = commentsUIService.GetAddCommentsInfo(new CommentingEnabledWebpageBuilder().Build());

            info.View.Should().Be("Login");
        }

        [Fact]
        public void IfGuestCommentsAreTurnedOffAndCurrentUserIsNullModelShouldBeLoginModel()
        {
            CurrentRequestData.CurrentUser = null;
            CommentsUIService commentsUIService = new CommentsUIServiceBuilder().WithGuestCommentsDisabled().Build();

            var info = commentsUIService.GetAddCommentsInfo(new CommentingEnabledWebpageBuilder().Build());

            info.Model.Should().BeOfType<LoginModel>();
        }

        [Fact]
        public void IfGuestCommentsAreTurnedOffAndCurrentUserIsLoggedInViewNameShouldBeLoggedIn()
        {
            CurrentRequestData.CurrentUser = new User();
            CommentsUIService commentsUIService = new CommentsUIServiceBuilder().WithGuestCommentsDisabled().Build();

            var info = commentsUIService.GetAddCommentsInfo(new CommentingEnabledWebpageBuilder().Build());

            info.View.Should().Be("LoggedIn");
        }

        [Fact]
        public void IfGuestCommentsAreTurnedOffAndCurrentUserIsLoggedInModelShouldBeLoggedInUserAddCommentModel()
        {
            CurrentRequestData.CurrentUser = new User();
            CommentsUIService commentsUIService = new CommentsUIServiceBuilder().WithGuestCommentsDisabled().Build();

            var info = commentsUIService.GetAddCommentsInfo(new CommentingEnabledWebpageBuilder().Build());

            info.Model.Should().BeOfType<LoggedInUserAddCommentModel>();
        }

        [Fact]
        public void IfGuestCommentsAreTurnedOffAndCurrentUserIsLoggedInModelShouldHaveWebpageIdAssigned()
        {
            CurrentRequestData.CurrentUser = new User();
            CommentsUIService commentsUIService = new CommentsUIServiceBuilder().WithGuestCommentsDisabled().Build();

            var info = commentsUIService.GetAddCommentsInfo(new CommentingEnabledWebpageBuilder().WithId(123).Build());

            info.Model.As<LoggedInUserAddCommentModel>().WebpageId.Should().Be(123);
        }

        [Fact]
        public void IfGuestCommentsAreTurnedOnAndCurrentUserIsLoggedInViewNameShouldBeLoggedIn()
        {
            CurrentRequestData.CurrentUser = new User();
            CommentsUIService commentsUIService = new CommentsUIServiceBuilder().WithGuestCommentsEnabled().Build();

            var info = commentsUIService.GetAddCommentsInfo(new CommentingEnabledWebpageBuilder().Build());

            info.View.Should().Be("LoggedIn");
        }

        [Fact]
        public void IfGuestCommentsAreTurnedOnAndCurrentUserIsLoggedInModelShouldBeLoggedInUserAddCommentModel()
        {
            CurrentRequestData.CurrentUser = new User();
            CommentsUIService commentsUIService = new CommentsUIServiceBuilder().WithGuestCommentsEnabled().Build();

            var info = commentsUIService.GetAddCommentsInfo(new CommentingEnabledWebpageBuilder().Build());

            info.Model.Should().BeOfType<LoggedInUserAddCommentModel>();
        }

        [Fact]
        public void IfGuestCommentsAreTurnedOnAndCurrentUserIsNotLoggedInViewNameShouldBeGuest()
        {
            CurrentRequestData.CurrentUser = null;
            CommentsUIService commentsUIService = new CommentsUIServiceBuilder().WithGuestCommentsEnabled().Build();

            var info = commentsUIService.GetAddCommentsInfo(new CommentingEnabledWebpageBuilder().Build());

            info.View.Should().Be("Guest");
        }

        [Fact]
        public void IfGuestCommentsAreTurnedOnAndCurrentUserIsNotLoggedInModelShouldBeGuestAddCommentModel()
        {
            CurrentRequestData.CurrentUser = null;
            CommentsUIService commentsUIService = new CommentsUIServiceBuilder().WithGuestCommentsEnabled().Build();

            var info = commentsUIService.GetAddCommentsInfo(new CommentingEnabledWebpageBuilder().Build());

            info.Model.Should().BeOfType<GuestAddCommentModel>();
        }

        [Fact]
        public void IfGuestCommentsAreTurnedOnAndCurrentUserIsNotLoggedInModelShouldHaveWebpageIdSet()
        {
            CurrentRequestData.CurrentUser = null;
            CommentsUIService commentsUIService = new CommentsUIServiceBuilder().WithGuestCommentsEnabled().Build();
            var basicMappedWebpage = new CommentingEnabledWebpageBuilder().WithId(123).Build();

            var info = commentsUIService.GetAddCommentsInfo(basicMappedWebpage);

            info.Model.As<GuestAddCommentModel>().WebpageId.Should().Be(123);
        }
    }
}