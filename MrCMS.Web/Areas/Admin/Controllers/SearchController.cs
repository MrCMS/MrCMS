using System.Web.Mvc;
using MrCMS.Web.Areas.Admin.Models.Search;
using MrCMS.Web.Areas.Admin.Services.Search;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
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