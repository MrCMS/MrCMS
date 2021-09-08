using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Scheduling;
using MrCMS.TextSearch.Services;
using MrCMS.Web.Admin.Infrastructure.BaseControllers;
using MrCMS.Web.Admin.Infrastructure.Helpers;
using MrCMS.Web.Admin.Models;
using MrCMS.Web.Admin.Services;

namespace MrCMS.Web.Admin.Controllers
{
    public class TextSearchController : MrCMSAdminController
    {
        private readonly IAdHocJobScheduler _jobScheduler;
        private readonly IQuickSearcher _quickSearcher;

        public TextSearchController(IAdHocJobScheduler jobScheduler, IQuickSearcher quickSearcher)
        {
            _jobScheduler = jobScheduler;
            _quickSearcher = quickSearcher;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Refresh()
        {
            // _refreshTextSearchIndex.Refresh();
            await _jobScheduler.Schedule<RefreshTextSearchIndex>();
            
            TempData.AddSuccessMessage("Index refresh started");

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<JsonResult> QuickSearch(QuickSearchParams searchParams)
        {
            return Json(await _quickSearcher.QuickSearch(searchParams));
        }
    }
}