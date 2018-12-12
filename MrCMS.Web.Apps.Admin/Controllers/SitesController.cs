using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.Multisite;
using MrCMS.Models;
using MrCMS.Web.Apps.Admin.ModelBinders;
using MrCMS.Web.Apps.Admin.Models;
using MrCMS.Web.Apps.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Admin.Controllers
{
    public class SitesController : MrCMSAdminController
    {
        private readonly ISiteAdminService _siteAdminService;

        public SitesController(ISiteAdminService siteAdminService)
        {
            _siteAdminService = siteAdminService;
        }

        [HttpGet]
        [ActionName("Index")]
        public ViewResult Index_Get()
        {
            var sites = _siteAdminService.GetAllSites();
            return View("Index", sites);
        }

        [HttpGet]
        [ActionName("Add")]
        public ViewResult Add_Get()
        {
            return View();
        }

        [HttpPost]
        public RedirectToActionResult Add(AddSiteModel model,
            [ModelBinder(typeof(GetSiteCopyOptionsModelBinder))]
            List<SiteCopyOption> options)
        {
            _siteAdminService.AddSite(model, options);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [ActionName("Edit")]
        public ViewResult Edit_Get(int id)
        {
            return View(_siteAdminService.GetEditModel(id));
        }

        [HttpPost]
        public RedirectToActionResult Edit(UpdateSiteModel model)
        {
            _siteAdminService.SaveSite(model);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [ActionName("Delete")]
        public PartialViewResult Delete_Get(int id)
        {
            return PartialView(_siteAdminService.GetEditModel(id));
        }

        [HttpPost]
        public RedirectToActionResult Delete(int id)
        {
            _siteAdminService.DeleteSite(id);
            return RedirectToAction("Index");
        }
    }
}