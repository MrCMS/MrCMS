using System;
using System.Web.Mvc;
using MrCMS.Web.Areas.Admin.ModelBinders;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class UrlGeneratorSettingsController : MrCMSAdminController
    {
        private readonly IUrlGeneratorSettingsAdminService _service;

        public UrlGeneratorSettingsController(IUrlGeneratorSettingsAdminService service)
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
            return PartialView(_service.GetInfo(type));
        }

        [HttpPost]
        public RedirectToRouteResult Set(DefaultGeneratorInfo info)
        {
            _service.SetDefault(info);
            return RedirectToAction("Index");
        }
    }
}