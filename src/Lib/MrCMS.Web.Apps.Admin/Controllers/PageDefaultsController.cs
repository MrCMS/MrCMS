using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Admin.ModelBinders;
using MrCMS.Web.Apps.Admin.Models;
using MrCMS.Web.Apps.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Admin.Controllers
{
    public class PageDefaultsController : MrCMSAdminController
    {
        private readonly IPageDefaultsAdminService _service;

        public PageDefaultsController(IPageDefaultsAdminService service)
        {
            _service = service;
        }

        public ViewResult Index()
        {
            return View(_service.GetAll());
        }

        [HttpGet]
        public async Task<PartialViewResult> Set(string type)
        {
            var webpageType = TypeHelper.GetTypeByName(type);
            ViewData["url-generator-options"] =await _service.GetUrlGeneratorOptions(webpageType);
            ViewData["layout-options"] = await _service.GetLayoutOptions();
            return PartialView(await _service.GetInfo(webpageType));
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