using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Apps.Admin.Models;
using MrCMS.Web.Apps.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Admin.Controllers
{
    public class NavigationController : MrCMSAdminController
    {
        private readonly ITreeNavService _treeNavService;

        public NavigationController(ITreeNavService treeNavService)
        {
            _treeNavService = treeNavService;
        }

        public PartialViewResult WebSiteTree(int? id)
        {
            AdminTree admintree = _treeNavService.GetWebpageNodes(id);
            return PartialView("TreeList", admintree);
        }

        public PartialViewResult MediaTree(int? id)
        {
            AdminTree admintree = _treeNavService.GetMediaCategoryNodes(id);
            return PartialView("TreeList", admintree);
        }

        public PartialViewResult LayoutTree(int? id)
        {
            AdminTree admintree = _treeNavService.GetLayoutNodes(id);
            return PartialView("TreeList", admintree);
        }
    }
}