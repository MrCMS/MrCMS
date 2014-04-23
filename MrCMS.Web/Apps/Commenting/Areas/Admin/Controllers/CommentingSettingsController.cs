using System.Web.Mvc;
using MrCMS.Settings;
using MrCMS.Web.Apps.Commenting.ModelBinders;
using MrCMS.Web.Apps.Commenting.Services;
using MrCMS.Web.Apps.Commenting.Settings;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Commenting.Areas.Admin.Controllers
{
    public class CommentingSettingsController : MrCMSAppAdminController<CommentingApp>
    {
        private readonly ICommentingSettingsAdminService _commentingSettingsAdminService;

        public CommentingSettingsController(ICommentingSettingsAdminService commentingSettingsAdminService)
        {
            _commentingSettingsAdminService = commentingSettingsAdminService;
        }

        [HttpGet]
        public ViewResult Index()
        {
            ViewData["page-types"] = _commentingSettingsAdminService.GetAllPageTypes();
            ViewData["comment-approval-types"] = _commentingSettingsAdminService.GetCommentApprovalTypes();
            return View(_commentingSettingsAdminService.GetSettings());
        }

        [HttpPost]
        public RedirectToRouteResult Index([IoCModelBinder(typeof(CommentingSettingsModelBinder))]CommentingSettings newSettings)
        {
            _commentingSettingsAdminService.UpdateSettings(newSettings);
            return RedirectToAction("Index");
        }
    }
}