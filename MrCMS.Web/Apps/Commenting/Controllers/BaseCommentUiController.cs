using System.Web.Mvc;
using MrCMS.Web.Apps.Commenting.Models;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Commenting.Controllers
{
    public abstract class BaseCommentUiController : MrCMSAppUIController<CommentingApp>
    {
        protected ActionResult RedirectToPage(ICommentResponseInfo response)
        {
            TempData["comment-response-info"] = response;
            return
                Redirect(string.IsNullOrWhiteSpace(response.RedirectUrl)
                    ? Referrer != null
                        ? Referrer.ToString()
                        : "~/"
                    : response.RedirectUrl);
        }
    }
}