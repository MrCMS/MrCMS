using FakeItEasy;
using MrCMS.Web.Apps.Commenting.Areas.Admin.Controllers;
using MrCMS.Web.Apps.Commenting.Services;

namespace MrCMS.Commenting.Tests.Admin.Controllers.CommentingSettingsControllerTests
{
    public class CommentingSettingsControllerBuilder
    {
        private ICommentingSettingsAdminService _commentingSettingsAdminService = A.Fake<ICommentingSettingsAdminService>();

        public CommentingSettingsControllerBuilder WithAdminService(ICommentingSettingsAdminService commentingSettingsAdminService)
        {
            _commentingSettingsAdminService = commentingSettingsAdminService;
            return this;
        }

        public CommentingSettingsController Build()
        {
            return new CommentingSettingsController(_commentingSettingsAdminService);
        }
    }
}