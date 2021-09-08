using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Models;
using MrCMS.Web.Admin.Infrastructure.BaseControllers;
using MrCMS.Web.Admin.Services;

namespace MrCMS.Web.Admin.Controllers
{
    public class TagPageController : MrCMSAdminController
    {
        private readonly ITagPageAdminService _adminService;
        private readonly IWebpageAdminService _webpageAdminService;

        public TagPageController(ITagPageAdminService adminService, IWebpageAdminService webpageAdminService)
        {
            _adminService = adminService;
            _webpageAdminService = webpageAdminService;
        }

        [HttpGet]
        public async Task<PartialViewResult> Documents(int id)
        {
            var page = await _webpageAdminService.GetWebpage(id);
            ViewData["webpages"] = await _adminService.GetWebpages(page as TagPage);

            return PartialView(page);
        }

        public async Task<JsonResult> Search(string term)
        {
            IList<AutoCompleteResult> result = await _adminService.Search(term);

            return Json(result);
        }

        public async Task<JsonResult> SearchPaged(string term, int page = 1)
        {
            var results = await _adminService.SearchPaged(term, page);

            return Json(new
            {
                Total = results.TotalItemCount, Items = results.ToList()
            });
        }
        public async Task<JsonResult> Info(int id)
        {
            return Json(await _adminService.GetInfo(id));
        }
    }
}