using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Admin.Models;
using MrCMS.Web.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Admin.Controllers
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