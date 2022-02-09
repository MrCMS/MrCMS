using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Admin.Infrastructure.BaseControllers;
using MrCMS.Web.Admin.Models;
using MrCMS.Web.Admin.Services;

namespace MrCMS.Web.Admin.Controllers
{
    public class PageTemplateController : MrCMSAdminController
    {
        private readonly IPageTemplateAdminService _service;

        public PageTemplateController(IPageTemplateAdminService service)
        {
            _service = service;
        }

        public async Task<ViewResult> Index(PageTemplateSearchQuery query)
        {
            ViewData["results"] = await _service.Search(query);
            return View(query);
        }

        [HttpGet]
        public async Task<ViewResult> Add()
        {
            ViewData["page-type-options"] = _service.GetPageTypeOptions();
            ViewData["layout-options"] = await _service.GetLayoutOptions();
            ViewData["url-generator-options"] = _service.GetUrlGeneratorOptions(null);
            return View();
        }

        [HttpPost]
        public async Task<RedirectToActionResult> Add(AddPageTemplateModel model)
        {
            await _service.Add(model);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ViewResult> Edit(int id)
        {
            var model = await _service.GetEditModel(id);
            ViewData["layout-options"] = await _service.GetLayoutOptions();
            ViewData["url-generator-options"] = _service.GetUrlGeneratorOptions(model.PageType);
            return View(model);
        }

        [HttpPost]
        [ActionName("Edit")]
        public async Task<ActionResult> Edit_POST(UpdatePageTemplateModel model)
        {
            await _service.Update(model);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<PartialViewResult> Delete(int id)
        {
            var model = await _service.GetEditModel(id);
            return PartialView(model);
        }

        [HttpPost]
        [ActionName("Delete")]
        public async Task<RedirectToActionResult> Delete_POST(int id)
        {
            await _service.Delete(id);
            return RedirectToAction("Index");
        }

        public JsonResult GetUrlGeneratorOptions(string type)
        {
            return Json(_service.GetUrlGeneratorOptions(type));
        }
    }
}