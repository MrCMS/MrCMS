using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Commenting.Models;

namespace MrCMS.Commenting.Tests.Services.CommentsUIServiceTests
{
    public class LoggedInAddCommentModelBuilder
    {
        private int _webpageId;
        private string _message;

        public LoggedInUserAddCommentModel Build()
        {
            return new LoggedInUserAddCommentModel
                       {
                           Message = _message,
                           WebpageId = _webpageId
                       };
        }

        public LoggedInAddCommentModelBuilder WithMessage(string message)
        {
            _message = message;
            return this;
        }
        public LoggedInAddCommentModelBuilder ForWebpage(Webpage webpage)
        {
            if (webpage != null)
            {
                _webpageId = webpage.Id;
            }
            return this;
        }
    }
}