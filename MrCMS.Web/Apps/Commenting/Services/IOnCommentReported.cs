using MrCMS.Web.Apps.Commenting.Entities;

namespace MrCMS.Web.Apps.Commenting.Services
{
    public interface IOnCommentReported
    {
        void CommentReported(Comment comment);
    }
}