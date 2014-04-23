using FluentAssertions;
using Xunit;

namespace MrCMS.Commenting.Tests.Services.CommentAdminServiceTests
{
    public class GetApprovalOptions
    {
        [Fact]
        public void ShouldHave4Options()
        {
            var commentAdminService = new CommentAdminServiceBuilder().Build();

            var items = commentAdminService.GetApprovalOptions();

            items.Should().HaveCount(4);
        }

        [Fact]
        public void FirstItemShouldBeAny()
        {
            var commentAdminService = new CommentAdminServiceBuilder().Build();

            var items = commentAdminService.GetApprovalOptions();

            items[0].Text.Should().Be("Any");
        }

        [Fact]
        public void SecondItemShouldBePending()
        {
            var commentAdminService = new CommentAdminServiceBuilder().Build();

            var items = commentAdminService.GetApprovalOptions();

            items[1].Text.Should().Be("Pending");
        }

        [Fact]
        public void ThirdItemShouldBeRejected()
        {
            var commentAdminService = new CommentAdminServiceBuilder().Build();

            var items = commentAdminService.GetApprovalOptions();

            items[2].Text.Should().Be("Rejected");
        }

        [Fact]
        public void FourthItemShouldBeApproved()
        {
            var commentAdminService = new CommentAdminServiceBuilder().Build();

            var items = commentAdminService.GetApprovalOptions();

            items[3].Text.Should().Be("Approved");
        }
    }
}