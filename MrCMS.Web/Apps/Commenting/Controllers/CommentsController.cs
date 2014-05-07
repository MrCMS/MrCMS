using System;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Commenting.Entities;
using MrCMS.Web.Apps.Commenting.ModelBinders;
using MrCMS.Web.Apps.Commenting.Models;
using MrCMS.Web.Apps.Commenting.Services;
using MrCMS.Website.Binders;

namespace MrCMS.Web.Apps.Commenting.Controllers
{
    public class CommentsController : BaseCommentUiController
    {
        private readonly ICommentsUIService _commentsUiService;

        public CommentsController(ICommentsUIService commentsUIService)
        {
            _commentsUiService = commentsUIService;
        }

        public ActionResult Add(Webpage webpage)
        {
            CommentsViewInfo info = _commentsUiService.GetAddCommentsInfo(webpage);
            return ReturnView(info);
        }

        public ActionResult Show(Webpage webpage)
        {
            CommentsViewInfo info = _commentsUiService.GetShowCommentsInfo(webpage);
            return ReturnView(info);
        }

        public ActionResult Votes(Comment comment)
        {
            return PartialView(comment);
        }

        public ActionResult ReplyTo(Comment comment)
        {
            CommentsViewInfo info = _commentsUiService.GetReplyToInfo(comment);
            return ReturnView(info);
        }

        [HttpPost]
        public ActionResult Guest([IoCModelBinder(typeof(SetIPAddressModelBinder))]GuestAddCommentModel model)
        {
            if (ModelState.IsValid)
            {
                PostCommentResponse response = _commentsUiService.AddGuestComment(model);
                return RedirectToPage(response);
            }
            return RedirectToPage(new PostCommentResponse { Valid = false, RedirectUrl = Referrer.ToString() });
        }

        [HttpPost]
        public ActionResult LoggedIn([IoCModelBinder(typeof(SetIPAddressModelBinder))]LoggedInUserAddCommentModel model)
        {
            if (ModelState.IsValid)
            {
                PostCommentResponse response = _commentsUiService.AddLoggedInComment(model);
                return RedirectToPage(response);
            }
            return RedirectToPage(new PostCommentResponse { Valid = false, RedirectUrl = Referrer.ToString() });
        }

        private ActionResult ReturnView(CommentsViewInfo info)
        {
            if (info.Disabled)
                return new EmptyResult();
            ViewData = info.ViewData;
            return PartialView(info.View, info.Model);
        }
    }
}