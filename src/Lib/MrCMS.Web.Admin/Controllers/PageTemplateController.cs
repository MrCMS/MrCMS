using System;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Web.Admin.Models;
using MrCMS.Web.Admin.Services;
using MrCMS.Web.Admin.ModelBinders;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Admin.Controllers
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
        public RedirectToActionResult Add(AddPageTemplateModel model)
        {
            _service.Add(model);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public PartialViewResult Edit(int id)
        {
            var model = _service.GetEditModel(id);
            ViewData["layout-options"] = _service.GetLayoutOptions();
            ViewData["url-generator-options"] = _service.GetUrlGeneratorOptions(model.PageType);
            return PartialView(model);
        }

        [HttpPost]
        [ActionName("Edit")]
        public RedirectToActionResult Edit_POST(UpdatePageTemplateModel model)
        {
            _service.Update(model);
            return RedirectToAction("Index");
        }

        public JsonResult GetUrlGeneratorOptions(string type)
        {
            return Json(_service.GetUrlGeneratorOptions(type));
        }
    }
}