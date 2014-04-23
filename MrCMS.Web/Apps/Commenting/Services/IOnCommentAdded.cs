using MrCMS.Web.Apps.Commenting.Entities;

namespace MrCMS.Web.Apps.Commenting.Services
{
    public interface IOnCommentAdded
    {
        void CommentAdded(Comment comment);
    }
}