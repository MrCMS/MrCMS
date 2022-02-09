using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Admin.Infrastructure.BaseControllers;
using MrCMS.Web.Admin.Models;
using MrCMS.Web.Admin.Services;

namespace MrCMS.Web.Admin.Controllers
{
    public class WebpageSearchController : MrCMSAdminController
    {
        private readonly IWebpageAdminSearchService _webpageAdminSearchService;

        public WebpageSearchController(IWebpageAdminSearchService webpageAdminSearchService)
        {
            _webpageAdminSearchService = webpageAdminSearchService;
        }

        public async Task<ViewResult> Search(WebpageSearchQuery searchQuery)
        {
            ViewData["results"] = await _webpageAdminSearchService.Search(searchQuery);
            return View(searchQuery);
        }

        public async Task<PartialViewResult> Results(WebpageSearchQuery searchQuery)
        {
            ViewData["results"] = await _webpageAdminSearchService.Search(searchQuery);
            return PartialView(searchQuery);
        }
    }
}