using System.Web.Mvc;
using MrCMS.Web.Apps.Commenting.ModelBinders;
using MrCMS.Web.Apps.Commenting.Models;
using MrCMS.Web.Apps.Commenting.Services;
using MrCMS.Website.Binders;

namespace MrCMS.Web.Apps.Commenting.Controllers
{
    public class CommentVotingController : BaseCommentUiController
    {
        private readonly ICommentVotingUiService _commentVotingUiService;

        public CommentVotingController(ICommentVotingUiService commentVotingUiService)
        {
            _commentVotingUiService = commentVotingUiService;
        }

        [HttpPost]
        public ActionResult Upvote([IoCModelBinder(typeof(SetIPAddressModelBinder))]VoteModel voteModel)
        {
            var response = _commentVotingUiService.Upvote(voteModel);
            if (Request.IsAjaxRequest())
            {
                return Json(response.IsSuccess());
            }
            return RedirectToPage(response);
        }

        [HttpPost]
        public ActionResult Downvote([IoCModelBinder(typeof(SetIPAddressModelBinder))]VoteModel voteModel)
        {
            var response = _commentVotingUiService.Downvote(voteModel);
            return Request.IsAjaxRequest() ? Json(response.IsSuccess()) : RedirectToPage(response);
        }
        
        
    }
}