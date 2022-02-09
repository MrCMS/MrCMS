using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Admin.Infrastructure.BaseControllers;
using MrCMS.Web.Admin.Infrastructure.Helpers;
using MrCMS.Website;

namespace MrCMS.Web.Admin.Controllers
{
    public class ClearCachesController : MrCMSAdminController
    {
        private readonly IClearCachesService _clearCachesService;

        public ClearCachesController(IClearCachesService clearCachesService)
        {
            _clearCachesService = clearCachesService;
        }

        [HttpGet]
        public ViewResult Index()
        {
            ViewData["cleared"] = TempData["cleared"];
            return View();
        }

        [HttpPost]
        [ActionName("Index")]
        public RedirectToActionResult Index_POST()
        {
            _clearCachesService.ClearCache();
            TempData.AddSuccessMessage("Caches cleared");
            return RedirectToAction("Index");
        }

        [HttpPost]
        public RedirectToActionResult HighPriority()
        {
            _clearCachesService.ClearHighPriorityCache();
            TempData.AddSuccessMessage("High priority cache cleared");
            return RedirectToAction("Index");
        }
    }
}