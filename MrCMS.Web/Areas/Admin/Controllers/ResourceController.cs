using System.Web.Mvc;
using MrCMS.Entities.Resources;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
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
            return View(searchQuery);
        }

        [HttpGet]
        public ViewResult Add(string key)
        {
            ViewData["language-options"] = _stringResourceAdminService.GetLanguageOptions(key);
            return View(new StringResource {Key = key});
        }

        [HttpPost]
        [ActionName("Add")]
        public RedirectToRouteResult Add_POST(StringResource resource)
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
        public RedirectToRouteResult Edit_POST(StringResource resource)
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
        public RedirectToRouteResult Delete_POST(StringResource resource)
        {
            _stringResourceAdminService.Delete(resource);
            return RedirectToAction("Index");
        }
    }
}