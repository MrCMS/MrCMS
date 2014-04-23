using FakeItEasy;
using FluentAssertions;
using MrCMS.Settings;
using MrCMS.Web.Apps.Commenting.Services;
using MrCMS.Web.Apps.Commenting.Settings;
using Xunit;

namespace MrCMS.Commenting.Tests.Admin.Controllers.CommentingSettingsControllerTests
{
    public class IndexPost
    {
        [Fact]
        public void ShouldCallUpdateSettings()
        {
            var commentingSettings = new CommentingSettings();
            var adminService = A.Fake<ICommentingSettingsAdminService>();
            var commentingSettingsController = new CommentingSettingsControllerBuilder().WithAdminService(adminService).Build();

            commentingSettingsController.Index(commentingSettings);

            A.CallTo(() => adminService.UpdateSettings(commentingSettings)).MustHaveHappened();
        }

        [Fact]
        public void ShouldRedirectToIndex()
        {
            var commentingSettingsController = new CommentingSettingsControllerBuilder().Build();
            var redirectToRouteResult = commentingSettingsController.Index(new CommentingSettings());

            redirectToRouteResult.RouteValues["action"].Should().Be("Index");
        }
    }
}