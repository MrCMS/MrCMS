using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Admin.Helpers;
using MrCMS.Web.Apps.Admin.Models;
using MrCMS.Web.Apps.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Admin.Controllers
{
    public class MergeWebpageController : MrCMSAdminController
    {
        private readonly IMergeWebpageAdminService _mergeWebpageAdminService;

        public MergeWebpageController(IMergeWebpageAdminService mergeWebpageAdminService)
        {
            _mergeWebpageAdminService = mergeWebpageAdminService;
        }

        [HttpGet]
        public ViewResult Index(Webpage webpage)
        {
            ViewData["valid-parents"] = _mergeWebpageAdminService.GetValidParents(webpage);
            ViewData["webpage"] = webpage;

            return View(_mergeWebpageAdminService.GetModel(webpage));
        }

        [HttpPost]
        public RedirectToActionResult Index(MergeWebpageModel model)
        {
            var validationResult = _mergeWebpageAdminService.Validate(model);
            if (!validationResult.Success)
            {
                TempData.ErrorMessages().Add(validationResult.Message);
                return RedirectToAction("Index", new { id = model.Id });
            }

            return RedirectToAction("Confirm", model);
        }

        public ViewResult Confirm(MergeWebpageModel model)
        {
            ViewData["confirmation-model"] = _mergeWebpageAdminService.GetConfirmationModel(model);
            return View(model);
        }

        [HttpPost, ActionName("Confirm")]
        public async Task<RedirectToActionResult> Confirm_POST(MergeWebpageModel model)
        {
            var validationResult = _mergeWebpageAdminService.Validate(model);
            if (!validationResult.Success)
            {
                TempData.ErrorMessages().Add(validationResult.Message);
                return RedirectToAction("Index", new { model.Id });
            }

            var confirmResult = await _mergeWebpageAdminService.Confirm(model);
            if (!confirmResult.Success)
            {
                TempData.ErrorMessages().Add(confirmResult.Message);
                return RedirectToAction("Index", new { model.Id });
            }

            TempData.SuccessMessages().Add(confirmResult.Message);
            return RedirectToAction("Edit", "Webpage", new { id = model.MergeIntoId });
        }
    }
}