using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website.Controllers;
using System.Web.Mvc;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class MoveWebpageController : MrCMSAdminController
    {
        private readonly IMoveWebpageAdminService _moveWebpageAdminService;

        public MoveWebpageController(IMoveWebpageAdminService moveWebpageAdminService)
        {
            _moveWebpageAdminService = moveWebpageAdminService;
        }

        [HttpGet]
        public ViewResult Index(Webpage webpage)
        {
            ViewData["valid-parents"] = _moveWebpageAdminService.GetValidParents(webpage);
            ViewData["webpage"] = webpage;

            return View(_moveWebpageAdminService.GetModel(webpage));
        }

        [HttpPost]
        public RedirectToRouteResult Index(MoveWebpageModel model)
        {
            var validationResult = _moveWebpageAdminService.Validate(model);
            if (!validationResult.Success)
            {
                TempData.ErrorMessages().Add(validationResult.Message);
                return RedirectToAction("Index", new { id = model.Id });
            }

            return RedirectToAction("Confirm", model);
        }

        public ViewResult Confirm(MoveWebpageModel model)
        {
            ViewData["confirmation-model"] = _moveWebpageAdminService.GetConfirmationModel(model);
            return View(model);
        }

        [HttpPost, ActionName("Confirm")]
        public RedirectToRouteResult Confirm_POST(MoveWebpageModel model)
        {
            var validationResult = _moveWebpageAdminService.Validate(model);
            if (!validationResult.Success)
            {
                TempData.ErrorMessages().Add(validationResult.Message);
                return RedirectToAction("Index", new { model.Id });
            }

            var confirmResult = _moveWebpageAdminService.Confirm(model);
            if (!confirmResult.Success)
            {
                TempData.ErrorMessages().Add(confirmResult.Message);
                return RedirectToAction("Index", new { model.Id });
            }

            TempData.SuccessMessages().Add(confirmResult.Message);
            return RedirectToAction("Edit", "Webpage", new { model.Id });
        }
    }
}