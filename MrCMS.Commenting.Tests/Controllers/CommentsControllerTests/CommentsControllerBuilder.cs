using FakeItEasy;
using MrCMS.Web.Apps.Commenting.Controllers;
using MrCMS.Web.Apps.Commenting.Services;

namespace MrCMS.Commenting.Tests.Controllers.CommentsControllerTests
{
    public class CommentsControllerBuilder
    {
        private ICommentsUIService _commentsUIService = A.Fake<ICommentsUIService>();

        public CommentsController Build()
        {
            return new CommentsController(_commentsUIService);
        }

        public CommentsControllerBuilder WithService(ICommentsUIService commentsUIService)
        {
            _commentsUIService = commentsUIService;
            return this;
        }
    }
}