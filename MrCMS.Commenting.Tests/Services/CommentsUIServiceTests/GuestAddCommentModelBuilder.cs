using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Commenting.Models;

namespace MrCMS.Commenting.Tests.Services.CommentsUIServiceTests
{
    public class GuestAddCommentModelBuilder
    {
        private string _name;
        private string _message;
        private string _email;
        private int _webpageId;

        public GuestAddCommentModel Build()
        {
            return new GuestAddCommentModel
                       {
                           Name = _name,
                           Message = _message,
                           Email = _email,
                           WebpageId = _webpageId
                       };
        }

        public GuestAddCommentModelBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public GuestAddCommentModelBuilder WithEmail(string email)
        {
            _email = email;
            return this;
        }

        public GuestAddCommentModelBuilder WithMessage(string message)
        {
            _message = message;
            return this;
        }

        public GuestAddCommentModelBuilder ForWebpage(Webpage webpage)
        {
            if (webpage != null)
            {
                _webpageId = webpage.Id;
            }
            return this;
        }
    }
}