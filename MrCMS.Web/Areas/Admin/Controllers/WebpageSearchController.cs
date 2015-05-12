using System.Web.Mvc;
using System.Web.UI;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
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
            return View(searchQuery);
        }

        public PartialViewResult Results(WebpageSearchQuery searchQuery)
        {
            ViewData["results"] = _webpageAdminSearchService.Search(searchQuery);
            return PartialView(searchQuery);
        }
    }
}