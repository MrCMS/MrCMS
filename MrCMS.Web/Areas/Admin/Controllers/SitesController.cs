using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Multisite;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Website.Binders;
using NHibernate;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class SitesController : AdminController
    {
        private readonly ISession _session;
        private readonly ISitesService _sitesService;
        private readonly IConfigurationProvider _configurationProvider;

        public SitesController(ISession session, ISitesService sitesService, IConfigurationProvider configurationProvider)
        {
            _session = session;
            _sitesService = sitesService;
            _configurationProvider = configurationProvider;
        }

        [HttpGet]
        [ActionName("Index")]
        public ViewResult Index_Get()
        {
            var sites = _sitesService.GetAllSites();
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
            _sitesService.SaveSite(site);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [ActionName("Edit")]
        public ViewResult Edit_Get(Site site)
        {
            ViewData["Settings"] = _configurationProvider.GetAllISettings(site)
                .Select(settings =>
                {
                    if (settings != null)
                        settings.SetViewData(_session, ViewData);
                    return settings;
                }).ToList();
            return View(site);
        }

        [HttpPost]
        public RedirectToRouteResult Edit(Site site, [ModelBinder(typeof(SettingModelBinder))]List<ISettings> settings)
        {
            _sitesService.SaveSite(site);
            settings.ForEach(s => s.Save());
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
            _sitesService.DeleteSite(site);
            return RedirectToAction("Index");
        }
    }
}