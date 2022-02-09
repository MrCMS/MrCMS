using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Admin.Models;
using MrCMS.Web.Admin.Services;
using MrCMS.Web.Admin.Infrastructure.BaseControllers;
using MrCMS.Web.Admin.Infrastructure.Helpers;

namespace MrCMS.Web.Admin.Controllers
{
    public class MergeWebpageController : MrCMSAdminController
    {
        private readonly IMergeWebpageAdminService _mergeWebpageAdminService;
        private readonly IWebpageAdminService _webpageAdminService;

        public MergeWebpageController(IMergeWebpageAdminService mergeWebpageAdminService, IWebpageAdminService webpageAdminService)
        {
            _mergeWebpageAdminService = mergeWebpageAdminService;
            _webpageAdminService = webpageAdminService;
        }

        [HttpGet]
        public async Task<ViewResult> Index(int id)
        {
            var webpage = await _webpageAdminService.GetWebpage(id);
            ViewData["valid-parents"] = await _mergeWebpageAdminService.GetValidParents(webpage);
            ViewData["webpage"] = webpage;

            return View(_mergeWebpageAdminService.GetModel(webpage));
        }

        [HttpPost]
        public async Task<RedirectToActionResult> Index(MergeWebpageModel model)
        {
            var validationResult = await _mergeWebpageAdminService.Validate(model);
            if (!validationResult.Success)
            {
                TempData.AddErrorMessage(validationResult.Message);
                return RedirectToAction("Index", new { id = model.Id });
            }

            return RedirectToAction("Confirm", model);
        }

        public async Task<ViewResult> Confirm(MergeWebpageModel model)
        {
            ViewData["confirmation-model"] = await _mergeWebpageAdminService.GetConfirmationModel(model);
            return View(model);
        }

        [HttpPost, ActionName("Confirm")]
        public async Task<RedirectToActionResult> Confirm_POST(MergeWebpageModel model)
        {
            var validationResult = await _mergeWebpageAdminService.Validate(model);
            if (!validationResult.Success)
            {
                TempData.AddErrorMessage(validationResult.Message);
                return RedirectToAction("Index", new { model.Id });
            }

            var confirmResult = await _mergeWebpageAdminService.Confirm(model);
            if (!confirmResult.Success)
            {
                TempData.AddErrorMessage(confirmResult.Message);
                return RedirectToAction("Index", new { model.Id });
            }

            TempData.AddSuccessMessage(confirmResult.Message);
            return RedirectToAction("Edit", "Webpage", new { id = model.MergeIntoId });
        }
    }
}