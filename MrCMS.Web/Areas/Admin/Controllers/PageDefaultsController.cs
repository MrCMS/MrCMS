using System;
using System.Web.Mvc;
using MrCMS.Web.Areas.Admin.ModelBinders;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
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
        public PartialViewResult Set([IoCModelBinder(typeof(GetUrlGeneratorOptionsTypeModelBinder))] Type type)
        {
            ViewData["url-generator-options"] = _service.GetUrlGeneratorOptions(type);
            ViewData["layout-options"] = _service.GetLayoutOptions();
            return PartialView(_service.GetInfo(type));
        }

        [HttpPost]
        public RedirectToRouteResult Set(DefaultsInfo info)
        {
            _service.SetDefaults(info);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public RedirectToRouteResult EnableCache(string typeName)
        {
            _service.EnableCache(typeName);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public RedirectToRouteResult DisableCache(string typeName)
        {
            _service.DisableCache(typeName);
            return RedirectToAction("Index");
        }
    }
}