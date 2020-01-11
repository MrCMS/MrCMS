using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Admin.Helpers;
using MrCMS.Web.Apps.Admin.Models;
using MrCMS.Web.Apps.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Admin.Controllers
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
        public RedirectToActionResult Index(MoveWebpageModel model)
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
        public async Task<RedirectToActionResult> Confirm_POST(MoveWebpageModel model)
        {
            var validationResult = _moveWebpageAdminService.Validate(model);
            if (!validationResult.Success)
            {
                TempData.ErrorMessages().Add(validationResult.Message);
                return RedirectToAction("Index", new { model.Id });
            }

            var confirmResult = await _moveWebpageAdminService.Confirm(model);
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