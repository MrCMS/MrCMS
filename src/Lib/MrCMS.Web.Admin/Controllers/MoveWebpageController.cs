using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Admin.Models;
using MrCMS.Web.Admin.Services;
using MrCMS.Web.Admin.Infrastructure.BaseControllers;
using MrCMS.Web.Admin.Infrastructure.Helpers;

namespace MrCMS.Web.Admin.Controllers
{
    public class MoveWebpageController : MrCMSAdminController
    {
        private readonly IMoveWebpageAdminService _moveWebpageAdminService;
        private readonly IWebpageAdminService _webpageAdminService;

        public MoveWebpageController(IMoveWebpageAdminService moveWebpageAdminService,
            IWebpageAdminService webpageAdminService)
        {
            _moveWebpageAdminService = moveWebpageAdminService;
            _webpageAdminService = webpageAdminService;
        }

        [HttpGet]
        public async Task<ViewResult> Index(int id)
        {
            var webpage = await _webpageAdminService.GetWebpage(id);
            ViewData["valid-parents"] = await _moveWebpageAdminService.GetValidParents(webpage);
            ViewData["webpage"] = webpage;

            return View(_moveWebpageAdminService.GetModel(webpage));
        }

        [HttpPost]
        public async Task<RedirectToActionResult> Index(MoveWebpageModel model)
        {
            var validationResult = await _moveWebpageAdminService.Validate(model);
            if (!validationResult.Success)
            {
                TempData.AddErrorMessage(validationResult.Message);
                return RedirectToAction("Index", new {id = model.Id});
            }

            return RedirectToAction("Confirm", model);
        }

        public async Task<ViewResult> Confirm(MoveWebpageModel model)
        {
            ViewData["confirmation-model"] = await _moveWebpageAdminService.GetConfirmationModel(model);
            return View(model);
        }

        [HttpPost, ActionName("Confirm")]
        public async Task<RedirectToActionResult> Confirm_POST(MoveWebpageModel model)
        {
            var validationResult = await _moveWebpageAdminService.Validate(model);
            if (!validationResult.Success)
            {
                TempData.AddErrorMessage(validationResult.Message);
                return RedirectToAction("Index", new {model.Id});
            }

            var confirmResult = await _moveWebpageAdminService.Confirm(model);
            if (!confirmResult.Success)
            {
                TempData.AddErrorMessage(confirmResult.Message);
                return RedirectToAction("Index", new {model.Id});
            }

            TempData.AddSuccessMessage(confirmResult.Message);
            return RedirectToAction("Edit", "Webpage", new {model.Id});
        }
    }
}