using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Apps.Admin.Models;
using MrCMS.Web.Apps.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Admin.Controllers
{
    public class UniversalSearchController : MrCMSAdminController
    {
        private readonly IUniversalSearchIndexSearcher _searchIndexSearcher;

        public UniversalSearchController(IUniversalSearchIndexSearcher searchIndexSearcher)
        {
            _searchIndexSearcher = searchIndexSearcher;
        }

        [HttpGet]
        public JsonResult QuickSearch(QuickSearchParams searchParams)
        {
            return Json(_searchIndexSearcher.QuickSearch(searchParams));
        }
    }
}