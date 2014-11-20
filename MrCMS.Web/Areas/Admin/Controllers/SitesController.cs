using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Entities.Multisite;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.ModelBinders;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;
using MrCMS.Helpers;

namespace MrCMS.Web.Areas.Admin.Controllers
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
        public RedirectToRouteResult Add(Site site, [IoCModelBinder(typeof(GetSiteCopyOptionsModelBinder))] List<SiteCopyOption> options)
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
        public RedirectToRouteResult Edit(Site site)
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
        public RedirectToRouteResult Delete(Site site)
        {
            _siteAdminService.DeleteSite(site);
            return RedirectToAction("Index");
        }
    }
}