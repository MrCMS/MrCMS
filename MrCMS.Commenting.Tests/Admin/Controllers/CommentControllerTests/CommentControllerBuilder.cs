using FakeItEasy;
using MrCMS.Web.Apps.Commenting.Areas.Admin.Controllers;
using MrCMS.Web.Apps.Commenting.Services;

namespace MrCMS.Commenting.Tests.Admin.Controllers.CommentControllerTests
{
    public class CommentControllerBuilder
    {
        private ICommentAdminService _commentAdminService = A.Fake<ICommentAdminService>();

        public CommentController Build()
        {
            return new CommentController(_commentAdminService);
        }

        public CommentControllerBuilder WithAdminService(ICommentAdminService commentAdminService)
        {
            _commentAdminService = commentAdminService;
            return this;
        }
    }
}