using FluentAssertions;
using Xunit;

namespace MrCMS.Commenting.Tests.Services.CommentingSettingsAdminServiceTests
{
    public class GetCommentApprovalTypes
    {
        [Fact]
        public void ShouldHave3Values()
        {
            var commentingAdminService = new CommentingSettingsAdminServiceBuilder().Build();

            var commentApprovalTypes = commentingAdminService.GetCommentApprovalTypes();

            commentApprovalTypes.Should().HaveCount(3);
        }

        [Fact]
        public void FirstShouldBeNone()
        {
            var commentingAdminService = new CommentingSettingsAdminServiceBuilder().Build();

            var commentApprovalTypes = commentingAdminService.GetCommentApprovalTypes();

            commentApprovalTypes[0].Value.Should().Be("None");
        }

        [Fact]
        public void SecondShouldBeNone()
        {
            var commentingAdminService = new CommentingSettingsAdminServiceBuilder().Build();

            var commentApprovalTypes = commentingAdminService.GetCommentApprovalTypes();

            commentApprovalTypes[1].Value.Should().Be("Guest");
        }

        [Fact]
        public void ThirdShouldBeAll()
        {
            var commentingAdminService = new CommentingSettingsAdminServiceBuilder().Build();

            var commentApprovalTypes = commentingAdminService.GetCommentApprovalTypes();

            commentApprovalTypes[2].Value.Should().Be("All");
        }
    }
}