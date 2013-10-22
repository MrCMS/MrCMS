using System.Web.Mvc;
using MrCMS.Entities.Multisite;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Website.Controllers;
using MrCMS.Helpers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class SitesController : MrCMSAdminController
    {
        private readonly ISiteService _siteService;

        public SitesController(ISiteService siteService)
        {
            _siteService = siteService;
        }

        [HttpGet]
        [ActionName("Index")]
        public ViewResult Index_Get()
        {
            var sites = _siteService.GetAllSites();
            return View("Index", sites);
        }

        [HttpGet]
        [ActionName("Add")]
        public PartialViewResult Add_Get()
        {
            ViewData["other-sites"] = _siteService.GetAllSites()
                                                  .BuildSelectItemList(site => site.Name, site => site.Id.ToString(),
                                                                       emptyItemText: "Do not copy");

            return PartialView(new Site());
        }

        [HttpPost]
        public RedirectToRouteResult Add(Site site, SiteCopyOptions options)
        {
            _siteService.AddSite(site, options);
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
            _siteService.SaveSite(site);
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
            _siteService.DeleteSite(site);
            return RedirectToAction("Index");
        }
    }
}