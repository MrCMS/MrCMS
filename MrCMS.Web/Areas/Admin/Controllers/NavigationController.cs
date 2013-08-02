using System;
using System.Linq;
using System.Web.Mvc;
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

        public NavigationController(INavigationService service, ISiteService siteService)
        {
            _service = service;
            _siteService = siteService;
        }

        public PartialViewResult WebSiteTree()
        {
            return PartialView("WebsiteTreeList", _service.GetWebsiteTree());
        }

        public PartialViewResult MediaTree()
        {
            return PartialView("MediaTree", _service.GetMediaTree());
        }

        public PartialViewResult LayoutTree()
        {
            return PartialView("LayoutTree", _service.GetLayoutList());
        }

        public PartialViewResult NavLinks()
        {
            var items = TypeHelper.GetAllConcreteTypesAssignableFrom<IAdminMenuItem>().Select(Activator.CreateInstance).Cast<IAdminMenuItem>().OrderBy(item => item.DisplayOrder);
            return PartialView("NavLinks", items);
        }

        public ActionResult SiteList()
        {
            var allSites = _siteService.GetAllSites();

            if (allSites.Count == 1)
                return new EmptyResult();

            return PartialView(allSites.BuildSelectItemList(site => site.Name, site => string.Format("http://{0}/admin/", site.BaseUrl),
                                                            site => _siteService.GetCurrentSite() == site,
                                                            emptyItemText: null));
        }
    }
}