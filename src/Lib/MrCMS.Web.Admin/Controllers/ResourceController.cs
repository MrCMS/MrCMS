using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Admin.Infrastructure.BaseControllers;
using MrCMS.Web.Admin.Models;
using MrCMS.Web.Admin.Services;

namespace MrCMS.Web.Admin.Controllers
{
    public class ResourceController : MrCMSAdminController
    {
        private readonly IStringResourceAdminService _stringResourceAdminService;

        public ResourceController(IStringResourceAdminService stringResourceAdminService)
        {
            _stringResourceAdminService = stringResourceAdminService;
        }

        public async Task<ViewResult> Index(StringResourceSearchQuery searchQuery)
        {
            ViewData["results"] = await _stringResourceAdminService.Search(searchQuery);
            ViewData["language-options"] = await _stringResourceAdminService.SearchLanguageOptions();
            ViewData["site-options"] = await _stringResourceAdminService.SearchSiteOptions();
            return View(searchQuery);
        }

        [HttpGet]
        public async Task<ViewResult> ChooseSite(ChooseSiteParams chooseSiteParams)
        {
            ViewData["site-options"] = await _stringResourceAdminService.ChooseSiteOptions(chooseSiteParams);
            return View(chooseSiteParams);
        }

        [HttpGet]
        public async Task<ViewResult> Add(string key, int? id, bool language = false)
        {
            if (language)
                ViewData["language-options"] = await _stringResourceAdminService.GetLanguageOptions(key, id);

            return View(await _stringResourceAdminService.GetNewResource(key, id));
        }

        [HttpPost]
        [ActionName("Add")]
        public async Task<RedirectToActionResult> Add_POST(AddStringResourceModel model)
        {
            await _stringResourceAdminService.Add(model);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ViewResult> Edit(int id)
        {
            var resource = await _stringResourceAdminService.GetResource(id);
            ViewData["resource"] = resource;
            var model = _stringResourceAdminService.GetEditModel(resource);
            return View(model);
        }

        [HttpPost]
        [ActionName("Edit")]
        public async Task<RedirectToActionResult> Edit_POST(UpdateStringResourceModel model)
        {
            await _stringResourceAdminService.Update(model);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ViewResult> Delete(int id)
        {
            var resource = await _stringResourceAdminService.GetResource(id);
            return View(resource);
        }

        [HttpPost]
        [ActionName("Delete")]
        public async Task<RedirectToActionResult> Delete_POST(int id)
        {
            await _stringResourceAdminService.Delete(id);
            return RedirectToAction("Index");
        }
    }
}