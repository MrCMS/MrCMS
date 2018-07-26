using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.Multisite;
using MrCMS.Models;
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

            return View(new Site());
        }

        [HttpPost]
        public RedirectToActionResult Add(Site site,
            //[IoCModelBinder(typeof(GetSiteCopyOptionsModelBinder))]
            List<SiteCopyOption> options) // TODO: model-binding
        {
            _siteAdminService.AddSite(site, options);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [ActionName("Edit")]
        public ViewResult Edit_Get(Site site)
        {
            return View(site);
        }

        [HttpPost]
        public RedirectToActionResult Edit(Site site)
        {
            _siteAdminService.SaveSite(site);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [ActionName("Delete")]
        public PartialViewResult Delete_Get(Site site)
        {
            return PartialView(site);
        }

        [HttpPost]
        public RedirectToActionResult Delete(Site site)
        {
            _siteAdminService.DeleteSite(site);
            return RedirectToAction("Index");
        }
    }
}