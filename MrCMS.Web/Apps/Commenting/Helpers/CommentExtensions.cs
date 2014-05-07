using MrCMS.Web.Apps.Commenting.Entities;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Web.Apps.Commenting.Helpers
{
    public static class CommentExtensions
    {
        public static int Upvotes(this Comment comment)
        {
            return comment == null
                ? 0
                : MrCMSApplication.Get<ISession>()
                    .QueryOver<Vote>()
                    .Where(vote => vote.Comment.Id == comment.Id && vote.IsUpvote)
                    .Cacheable()
                    .RowCount();
        }
        public static int Downvotes(this Comment comment)
        {
            return comment == null
                ? 0
                : MrCMSApplication.Get<ISession>()
                    .QueryOver<Vote>()
                    .Where(vote => vote.Comment.Id == comment.Id && !vote.IsUpvote)
                    .Cacheable()
                    .RowCount();
        }
    }
}