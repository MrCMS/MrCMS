using MrCMS.Web.Apps.Commenting.Settings;
using Xunit;
using FluentAssertions;

namespace MrCMS.Commenting.Tests.Settings.CommentingSettingsTests
{
    public class Defaults
    {
        [Fact]
        public void AllowGuestCommentsShouldBeFalse()
        {
            var commentingSettings = new CommentSettingsBuilder().Build();

            commentingSettings.AllowGuestComments.Should().BeFalse();
        }

        [Fact]
        public void CommentApprovalShouldBeGuestsOnly()
        {
            var commentingSettings = new CommentSettingsBuilder().Build();

            commentingSettings.CommentApprovalType.Should().Be(CommentApprovalType.Guest);
        }

        [Fact]
        public void AllowVotingShouldBeTrue()
        {
            var commentingSettings = new CommentSettingsBuilder().Build();

            commentingSettings.AllowVoting.Should().BeTrue();
        }

        [Fact]
        public void NotifyEmailShouldBeEmptyString()
        {
            var commentingSettings = new CommentSettingsBuilder().Build();

            commentingSettings.NotifyCommentAddedEmail.Should().BeBlank();
        }

        //[Fact]
        //public void NumberOfCommentsToShowShouldBe10()
        //{
        //    var commentingSettings = new CommentSettingsBuilder().Build();

        //    commentingSettings.InitialNumberOfCommentsToShow.Should().Be(10);
        //}
    }
}