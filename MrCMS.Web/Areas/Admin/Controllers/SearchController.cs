using System.Web.Mvc;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Models.Search;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class SearchController : MrCMSAdminController
    {
        private readonly INavigationService _navigationService;
        private readonly IAdminWebpageSearchService _adminWebpageSearchService;

        public SearchController(INavigationService navigationService, IAdminWebpageSearchService adminWebpageSearchService)
        {
            _navigationService = navigationService;
            _adminWebpageSearchService = adminWebpageSearchService;
        }

        [HttpGet]
        public ActionResult Index(AdminWebpageSearchQuery model)
        {
            ViewData["parents"] = _navigationService.GetParentsList();
            ViewData["doc-types"] = _navigationService.GetDocumentTypes(model.Type);
            ViewData["results"] = _adminWebpageSearchService.Search(model);

            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetSearchResults(AdminWebpageSearchQuery model)
        {
            if (string.IsNullOrWhiteSpace(model.Term))
                return Json(new object());
            return Json(_adminWebpageSearchService.QuickSearch(model), JsonRequestBehavior.AllowGet);
        }

        
        public PartialViewResult GetBreadCrumb(int? parentId)
        {
            if (parentId.HasValue)
                return PartialView(_adminWebpageSearchService.GetBreadCrumb(parentId));
            return PartialView(null);
        }
    }
}