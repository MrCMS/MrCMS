using System;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Documents.Web;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Website.Controllers;
using MrCMS.Helpers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class NavigationController : MrCMSAdminController
    {
        private readonly INavigationService _service;
        private readonly ISiteService _siteService;
        private readonly ICurrentSiteLocator _currentSiteLocator;

        public NavigationController(INavigationService service, ISiteService siteService, ICurrentSiteLocator currentSiteLocator)
        {
            _service = service;
            _siteService = siteService;
            _currentSiteLocator = currentSiteLocator;
        }

        public PartialViewResult WebSiteTree(int? id)
        {
            var admintree = _service.GetNodes<Webpage>(id);
            admintree.RootContoller = "Webpage";
            return PartialView("TreeList", admintree);
        }

        public PartialViewResult MediaTree(int? id)
        {
            var admintree = _service.GetNodes<MediaCategory>(id);
            admintree.RootContoller = "MediaCategory";
            return PartialView("TreeList", admintree);
        }

        public PartialViewResult LayoutTree(int? id)
        {
            var admintree = _service.GetNodes<Layout>(id);
            admintree.RootContoller = "Layout";
            return PartialView("TreeList", admintree);
        }

        public PartialViewResult NavLinks()
        {
            return PartialView("NavLinks", _service.GetNavLinks());
        }

        public ActionResult SiteList()
        {
            var allSites = _siteService.GetAllSites();

            if (allSites.Count == 1)
                return new EmptyResult();

            return PartialView(allSites.BuildSelectItemList(site => site.Name, site => string.Format("http://{0}/admin/", site.BaseUrl),
                                                            site => _currentSiteLocator.GetCurrentSite() == site,
                                                            emptyItemText: null));
        }
    }
}