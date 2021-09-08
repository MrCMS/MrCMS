using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Admin.Infrastructure.BaseControllers;
using MrCMS.Web.Admin.Models;
using MrCMS.Web.Admin.Services;

namespace MrCMS.Web.Admin.Controllers
{
    public class NavigationController : MrCMSAdminController
    {
        private readonly ITreeNavService _treeNavService;

        public NavigationController(ITreeNavService treeNavService)
        {
            _treeNavService = treeNavService;
        }

        public async Task<PartialViewResult> WebSiteTree(int? id)
        {
            AdminTree adminTree = await _treeNavService.GetWebpageNodes(id);
            return PartialView("TreeList", adminTree);
        }

        public async Task<PartialViewResult> MediaTree(int? id)
        {
            AdminTree adminTree = await _treeNavService.GetMediaCategoryNodes(id);
            return PartialView("TreeList", adminTree);
        }

        public async Task<PartialViewResult> LayoutTree(int? id)
        {
            AdminTree adminTree = await _treeNavService.GetLayoutNodes(id);
            return PartialView("TreeList", adminTree);
        }
    }
}