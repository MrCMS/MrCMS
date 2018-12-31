using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Apps.Admin.Models;
using MrCMS.Web.Apps.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Admin.Controllers
{
    public class WebpageSearchController : MrCMSAdminController
    {
        private readonly IWebpageAdminSearchService _webpageAdminSearchService;

        public WebpageSearchController(IWebpageAdminSearchService webpageAdminSearchService)
        {
            _webpageAdminSearchService = webpageAdminSearchService;
        }

        public ViewResult Search(WebpageSearchQuery searchQuery)
        {
            ViewData["results"] = _webpageAdminSearchService.Search(searchQuery);
            return View(searchQuery);
        }

        public PartialViewResult Results(WebpageSearchQuery searchQuery)
        {
            ViewData["results"] = _webpageAdminSearchService.Search(searchQuery);
            return PartialView(searchQuery);
        }
    }
}