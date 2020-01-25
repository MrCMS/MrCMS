using System;
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
        public PartialViewResult Set(string type)
        {
            var webpageType = TypeHelper.GetTypeByName(type);
            ViewData["url-generator-options"] = _service.GetUrlGeneratorOptions(webpageType);
            ViewData["layout-options"] = _service.GetLayoutOptions();
            return PartialView(_service.GetInfo(webpageType));
        }

        [HttpPost]
        public RedirectToActionResult Set(DefaultsInfo info)
        {
            _service.SetDefaults(info);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public RedirectToActionResult EnableCache(string typeName)
        {
            _service.EnableCache(typeName);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public RedirectToActionResult DisableCache(string typeName)
        {
            _service.DisableCache(typeName);
            return RedirectToAction("Index");
        }
    }
}