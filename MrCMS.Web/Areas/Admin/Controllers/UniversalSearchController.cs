using System.Web.Mvc;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
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
            return Json(_searchIndexSearcher.QuickSearch(searchParams), JsonRequestBehavior.AllowGet);
        }
    }
}