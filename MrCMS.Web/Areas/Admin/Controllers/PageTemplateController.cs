using System;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Web.Areas.Admin.ModelBinders;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class PageTemplateController : MrCMSAdminController
    {
        private readonly IPageTemplateAdminService _service;

        public PageTemplateController(IPageTemplateAdminService service)
        {
            _service = service;
        }

        public ViewResult Index(PageTemplateSearchQuery query)
        {
            ViewData["results"] = _service.Search(query);
            return View(query);
        }

        [HttpGet]
        public PartialViewResult Add()
        {
            ViewData["page-type-options"] = _service.GetPageTypeOptions();
            ViewData["layout-options"] = _service.GetLayoutOptions();
            ViewData["url-generator-options"] = _service.GetUrlGeneratorOptions(null);
            return PartialView();
        }

        [HttpPost]
        public RedirectToRouteResult Add(PageTemplate template)
        {
            _service.Add(template);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public PartialViewResult Edit(PageTemplate template)
        {
            ViewData["layout-options"] = _service.GetLayoutOptions();
            ViewData["url-generator-options"] = _service.GetUrlGeneratorOptions(template.GetPageType());
            return PartialView(template);
        }

        [HttpPost]
        [ActionName("Edit")]
        public RedirectToRouteResult Edit_POST(PageTemplate template)
        {
            _service.Update(template);
            return RedirectToAction("Index");
        }

        public JsonResult GetUrlGeneratorOptions(
            [IoCModelBinder(typeof (GetUrlGeneratorOptionsTypeModelBinder))] Type type)
        {
            return Json(_service.GetUrlGeneratorOptions(type), JsonRequestBehavior.AllowGet);
        }
    }
}