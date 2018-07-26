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
        public ViewResult Add(string key, 
            //[IoCModelBinder(typeof(NullableEntityModelBinder))]
            Site site, bool language = false) // TODO: model-binding
        {
            if (language)
                ViewData["language-options"] = _stringResourceAdminService.GetLanguageOptions(key, site);

            return View(_stringResourceAdminService.GetNewResource(key, site));
        }

        [HttpPost]
        [ActionName("Add")]
        public RedirectToActionResult Add_POST(StringResource resource)
        {
            _stringResourceAdminService.Add(resource);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ViewResult Edit(StringResource resource)
        {
            return View(resource);
        }

        [HttpPost]
        [ActionName("Edit")]
        public RedirectToActionResult Edit_POST(StringResource resource)
        {
            _stringResourceAdminService.Update(resource);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ViewResult Delete(StringResource resource)
        {
            return View(resource);
        }

        [HttpPost]
        [ActionName("Delete")]
        public RedirectToActionResult Delete_POST(StringResource resource)
        {
            _stringResourceAdminService.Delete(resource);
            return RedirectToAction("Index");
        }
    }
}