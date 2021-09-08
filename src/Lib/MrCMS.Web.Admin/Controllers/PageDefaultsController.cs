using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Admin.Infrastructure.BaseControllers;
using MrCMS.Web.Admin.Models;
using MrCMS.Web.Admin.Services;

namespace MrCMS.Web.Admin.Controllers
{
    public class PageDefaultsController : MrCMSAdminController
    {
        private readonly IPageDefaultsAdminService _service;

        public PageDefaultsController(IPageDefaultsAdminService service)
        {
            _service = service;
        }

        public async Task<ViewResult> Index()
        {
            return View(await _service.GetAll());
        }

        [HttpGet]
        public async Task<PartialViewResult> Set(string type)
        {
            ViewData["url-generator-options"] =  _service.GetUrlGeneratorOptions(type);
            ViewData["layout-options"] = await _service.GetLayoutOptions();
            return PartialView(_service.GetInfo(type));
        }

        [HttpPost]
        public async Task<RedirectToActionResult> Set(DefaultsInfo info)
        {
            await _service.SetDefaults(info);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<RedirectToActionResult> EnableCache(string typeName)
        {
            await _service.EnableCache(typeName);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<RedirectToActionResult> DisableCache(string typeName)
        {
            await _service.DisableCache(typeName);
            return RedirectToAction("Index");
        }
    }
}