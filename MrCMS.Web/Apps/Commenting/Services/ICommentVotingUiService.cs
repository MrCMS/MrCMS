using MrCMS.Web.Apps.Commenting.Models;

namespace MrCMS.Web.Apps.Commenting.Services
{
    public interface ICommentVotingUiService
    {
        VoteResponse Upvote(VoteModel voteModel);
        VoteResponse Downvote(VoteModel voteModel);
    }
}