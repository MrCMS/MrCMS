using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.TextSearch.Services;
using MrCMS.Web.Admin.Infrastructure.BaseControllers;
using MrCMS.Web.Admin.Services.Search;

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
        public async Task<ActionResult> Index(ITextSearcher.PagedTextSearcherQuery model)
        {
            ViewData["results"] = await _adminSearchService.Search(model);
            ViewData["type-options"] = _adminSearchService.GetTypeOptions();

            return View(model);
        }
    }
}