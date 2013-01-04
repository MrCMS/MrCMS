using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.People;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Website;
using NHibernate;
using MrCMS.Helpers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class NavigationController : AdminController
    {
        private readonly INavigationService _service;
        private readonly IUserService _userService;
        private readonly ISiteService _siteService;

        public NavigationController(INavigationService service, IUserService userService, ISiteService siteService)
        {
            _service = service;
            _userService = userService;
            _siteService = siteService;
        }

        public PartialViewResult WebSiteTree()
        {
            var sites = _siteService.GetAllSites();
            var currentSite = _siteService.GetCurrentSite();
            return PartialView("WebsiteTreeList",
                               new WebsiteTreeListModel(sites.BuildSelectItemList(site => site.Name,
                                                                                  site => site.Id.ToString(),
                                                                                  site => site == currentSite,
                                                                                  emptyItemText: null),
                                                        sites.Select(site => _service.GetWebsiteTree(site)).ToList()));
        }

        public PartialViewResult MediaTree()
        {
            return PartialView("MediaTree", _service.GetMediaTree());
        }

        public PartialViewResult LayoutTree()
        {
            var sites = _siteService.GetAllSites();
            var currentSite = _siteService.GetCurrentSite();
            return PartialView("LayoutTree",
                               new LayoutTreeListModel(sites.BuildSelectItemList(site => site.Name,
                                                                                  site => site.Id.ToString(),
                                                                                  site => site == currentSite,
                                                                                  emptyItemText: null),
                                                        sites.Select(site => _service.GetLayoutList(site)).ToList()));
        }

        public PartialViewResult UserList()
        {
            return PartialView("UserList", _service.GetUserList());
        }

        public PartialViewResult NavLinks()
        {
            return PartialView("NavLinks");
        }

        [ChildActionOnly]
        public PartialViewResult LoggedInAs()
        {
            User user = _userService.GetCurrentUser(HttpContext);
            return PartialView(user);
        }
    }
}