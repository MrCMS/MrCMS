using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.Resources;
using MrCMS.Web.Apps.Admin.Models;
using MrCMS.Web.Apps.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Admin.Controllers
{
    public class ResourceController : MrCMSAdminController
    {
        private readonly IStringResourceAdminService _stringResourceAdminService;

        public ResourceController(IStringResourceAdminService stringResourceAdminService)
        {
            _stringResourceAdminService = stringResourceAdminService;
        }

        public ViewResult Index(StringResourceSearchQuery searchQuery)
        {
            ViewData["results"] = _stringResourceAdminService.Search(searchQuery);
            ViewData["language-options"] = _stringResourceAdminService.SearchLanguageOptions();
            ViewData["site-options"] = _stringResourceAdminService.SearchSiteOptions();
            return View(searchQuery);
        }

        [HttpGet]
        public ViewResult ChooseSite(ChooseSiteParams chooseSiteParams)
        {
            ViewData["site-options"] = _stringResourceAdminService.ChooseSiteOptions(chooseSiteParams);
            return View(chooseSiteParams);
        }

        [HttpGet]
        public ViewResult Add(string key, int? id, bool language = false)
        {
            if (language)
                ViewData["language-options"] = _stringResourceAdminService.GetLanguageOptions(key, id);

            return View(_stringResourceAdminService.GetNewResource(key, id));
        }

        [HttpPost]
        [ActionName("Add")]
        public RedirectToActionResult Add_POST(AddStringResourceModel model)
        {
            _stringResourceAdminService.Add(model);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ViewResult Edit(int id)
        {
            var resource =  _stringResourceAdminService.GetResource(id);
            ViewData["resource"] = resource;
            var model = _stringResourceAdminService.GetEditModel(resource);
            return View(model);
        }

        [HttpPost]
        [ActionName("Edit")]
        public RedirectToActionResult Edit_POST(UpdateStringResourceModel model)
        {
            _stringResourceAdminService.Update(model);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ViewResult Delete(StringResource resource)
        {
            return View(resource);
        }

        [HttpPost]
        [ActionName("Delete")]
        public RedirectToActionResult Delete_POST(int id)
        {
            _stringResourceAdminService.Delete(id);
            return RedirectToAction("Index");
        }
    }
}