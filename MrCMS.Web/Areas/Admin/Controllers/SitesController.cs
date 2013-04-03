using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Multisite;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;
using NHibernate;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class SitesController : MrCMSAdminController
    {
        private readonly ISiteService _siteService;
        private readonly IUserService _userService;
        private readonly IConfigurationProvider _configurationProvider;

        public SitesController(ISiteService siteService, IUserService userService, IConfigurationProvider configurationProvider)
        {
            _siteService = siteService;
            _userService = userService;
            _configurationProvider = configurationProvider;
        }

        [HttpGet]
        [ActionName("Index")]
        public ViewResult Index_Get()
        {
            ViewData["Settings"] = _configurationProvider.GetAllGlobalSettings();
            var sites = _siteService.GetAllSites();
            return View("Index", sites);
        }

        [HttpGet]
        [ActionName("Add")]
        public PartialViewResult Add_Get()
        {
            return PartialView(new Site());
        }

        [HttpPost]
        public RedirectToRouteResult Add(Site site)
        {
            _siteService.AddSite(site);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [ActionName("Edit")]
        public ViewResult Edit_Get(Site site)
        {
            ViewData["Users"] = _userService.GetAllUsers();

            return View(site);
        }

        [HttpPost]
        public RedirectToRouteResult Edit([IoCModelBinder(typeof(EditSiteModelBinder))] Site site)
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