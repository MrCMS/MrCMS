using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Admin.Models.Search;
using MrCMS.Web.Admin.Services.Search;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Admin.Controllers
{
    public class SearchController : MrCMSAdminController
    {
        private readonly IAdminSearchService _adminSearchService;

        public SearchController(IAdminSearchService adminSearchService)
        {
            _adminSearchService = adminSearchService;
        }

        [HttpGet]
        public ActionResult Index(AdminSearchQuery model)
        {
            ViewData["results"] = _adminSearchService.Search(model);

            return View(model);
        }
    }
}