using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Admin.ModelBinders;
using MrCMS.Web.Apps.Admin.Models;
using MrCMS.Web.Apps.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Admin.Controllers
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
        public async Task<RedirectToActionResult> Add(AddPageTemplateModel model)
        {
          await  _service.Add(model);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<PartialViewResult> Edit(int id)
        {
            var model = await _service.GetEditModel(id);
            ViewData["layout-options"] = _service.GetLayoutOptions();
            ViewData["url-generator-options"] = await _service.GetUrlGeneratorOptions(id);
            return PartialView(model);
        }

        [HttpPost]
        [ActionName("Edit")]
        public async Task<RedirectToActionResult> Edit_POST(UpdatePageTemplateModel model)
        {
            await _service.Update(model);
            return RedirectToAction("Index");
        }

        public JsonResult GetUrlGeneratorOptions(string type)
        {
            return Json(_service.GetUrlGeneratorOptions(type));
        }
    }
}