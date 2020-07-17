using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Admin.Models;
using MrCMS.Web.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Admin.Controllers
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