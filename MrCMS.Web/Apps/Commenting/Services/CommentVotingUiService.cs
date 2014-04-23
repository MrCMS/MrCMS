using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Commenting.Entities;
using MrCMS.Web.Apps.Commenting.Models;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Web.Apps.Commenting.Services
{
    public class CommentVotingUiService : ICommentVotingUiService
    {
        private readonly ISession _session;

        public CommentVotingUiService(ISession session)
        {
            _session = session;
        }

        public VoteResponse Upvote(VoteModel voteModel)
        {
            var comment = _session.Get<Comment>(voteModel.CommentId);
            if (comment == null)
            {
                return new VoteResponse
                       {
                           Type = CommentResponseType.Error,
                           Message = "Could not find comment to vote"
                       };
            }
            Webpage webpage = comment.Webpage;
            User currentUser = CurrentRequestData.CurrentUser;
            if (currentUser != null)
            {
                return RegisterLoggedInVote(voteModel, comment, currentUser, webpage, true);
            }
            return RegisterAnonymousVote(voteModel, comment, webpage, true);
        }

        public VoteResponse Downvote(VoteModel voteModel)
        {
            var comment = _session.Get<Comment>(voteModel.CommentId);
            if (comment == null)
            {
                return new VoteResponse
                       {
                           Type = CommentResponseType.Error,
                           Message = "Could not find comment to vote"
                       };
            }
            Webpage webpage = comment.Webpage;
            User currentUser = CurrentRequestData.CurrentUser;
            if (currentUser != null)
            {
                return RegisterLoggedInVote(voteModel, comment, currentUser, webpage, false);
            }
            return RegisterAnonymousVote(voteModel, comment, webpage, false);
        }

        private VoteResponse RegisterAnonymousVote(VoteModel voteModel, Comment comment, Webpage webpage, bool isUpvote)
        {
            if (comment.Votes.Any(v => v.IPAddress == voteModel.IPAddress))
            {
                return new VoteResponse
                       {
                           Type = CommentResponseType.Info,
                           Message = "Already voted",
                           RedirectUrl = "~/" + webpage.LiveUrlSegment
                       };
            }
            var vote = new Vote
                       {
                           IsUpvote = isUpvote,
                           Comment = comment,
                           IPAddress = voteModel.IPAddress
                       };
            comment.Votes.Add(vote);
            _session.Transact(session => session.Save(vote));
            return new VoteResponse
                   {
                       Message = "Vote registered",
                       RedirectUrl = "~/" + webpage.LiveUrlSegment,
                       Type = CommentResponseType.Success
                   };
        }

        private VoteResponse RegisterLoggedInVote(VoteModel voteModel, Comment comment, User currentUser,
            Webpage webpage, bool isUpvote)
        {
            if (comment.Votes.Any(v => v.IsUpvote == isUpvote && v.User == currentUser))
            {
                return new VoteResponse
                       {
                           Type = CommentResponseType.Info,
                           Message = "Already voted",
                           RedirectUrl = "~/" + webpage.LiveUrlSegment
                       };
            }
            List<Vote> oppositeVotes =
                comment.Votes.Where(v => v.IsUpvote != isUpvote && v.User == currentUser).ToList();
            _session.Transact(session => oppositeVotes.ForEach(v =>
                                                               {
                                                                   comment.Votes.Remove(v);
                                                                   session.Delete(v);
                                                               }));
            var vote = new Vote
                       {
                           IsUpvote = isUpvote,
                           User = currentUser,
                           Comment = comment,
                           IPAddress = voteModel.IPAddress
                       };
            comment.Votes.Add(vote);
            _session.Transact(session => session.Save(vote));
            return new VoteResponse
                   {
                       Message = "Vote registered",
                       RedirectUrl = "~/" + webpage.LiveUrlSegment,
                       Type = CommentResponseType.Success
                   };
        }
    }
}