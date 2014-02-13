using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Models.Search;
using MrCMS.Services;
using MrCMS.Services.Search;
using MrCMS.Web.Apps.Core.Models;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class SearchController : MrCMSAdminController
    {
        private readonly INavigationService _navigationService;
        private readonly IWebpageSearchService _webpageSearchService;

        public SearchController(INavigationService navigationService, IWebpageSearchService webpageSearchService)
        {
            _navigationService = navigationService;
            _webpageSearchService = webpageSearchService;
        }

        [HttpGet]
        public ActionResult Index(AdminWebpageSearchQuery model)
        {
            ViewData["parents"] = _navigationService.GetParentsList();
            ViewData["doc-types"] = _navigationService.GetDocumentTypes(model.Type);
            ViewData["results"] = _webpageSearchService.Search(model);

            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetSearchResults(AdminWebpageSearchQuery model)
        {
            if (string.IsNullOrWhiteSpace(model.Term))
                return Json(new object());
            return Json(_webpageSearchService.QuickSearch(model), JsonRequestBehavior.AllowGet);
        }

        
        public PartialViewResult GetBreadCrumb(int? parentId)
        {
            if (parentId.HasValue)
                return PartialView(_webpageSearchService.GetBreadCrumb(parentId));
            return PartialView(null);
        }
    }
}