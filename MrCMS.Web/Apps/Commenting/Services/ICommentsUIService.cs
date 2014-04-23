using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Commenting.Controllers;
using MrCMS.Web.Apps.Commenting.Entities;
using MrCMS.Web.Apps.Commenting.Models;

namespace MrCMS.Web.Apps.Commenting.Services
{
    public interface ICommentsUIService
    {
        CommentsViewInfo GetAddCommentsInfo(Webpage webpage);
        CommentsViewInfo GetShowCommentsInfo(Webpage webpage);
        CommentsViewInfo GetReplyToInfo(Comment comment);
        PostCommentResponse AddGuestComment(GuestAddCommentModel model);
        PostCommentResponse AddLoggedInComment(LoggedInUserAddCommentModel model);
    }
}