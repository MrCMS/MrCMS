using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using MrCMS.Entities.Documents;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Paging;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.Models;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class SearchController : MrCMSAdminController
    {
        private readonly ISearchService _searchService;
        private readonly INavigationService _navigationService;
        private readonly IDocumentService _documentService;

        public SearchController(ISearchService searchService, INavigationService navigationService, IDocumentService documentService)
        {
            _searchService = searchService;
            _navigationService = navigationService;
            _documentService = documentService;
        }

        [HttpGet]
        public ActionResult Index(string term, string type, int? parent, int page = 1)
        {
            ViewData["term"] = term;
            ViewData["parent-val"] = parent;
            ViewData["parents"] = _navigationService.GetParentsList();
            ViewData["type"] = type;
            ViewData["doc-types"] = _navigationService.GetDocumentTypes(type);

            var docs = GetDetailedResults(term, type, parent, page);
            return View(docs);
        }

        [HttpPost]
        [ActionName("Index")]
        public ActionResult IndexPost(string term, string type, int? parent)
        {
            return RedirectToAction("Index", new { term, type, parent });
        }

        //
        // GET: /Admin/Search/
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetSearchResults(string term, string type)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Json(new object());

            var docs = GetResults(term, type).Take(15).ToList();
            return Json(docs, JsonRequestBehavior.AllowGet);
        }

        private IPagedList<DetailedSearchResultModel> GetDetailedResults(string term, string type, int? parent, int page)
        {
            if (!string.IsNullOrWhiteSpace(type))
            {
                var typeByName = DocumentMetadataHelper.GetTypeByName(type);
                var searchResults = _searchService.GetType()
                                                    .GetMethodExt("SearchDocumentsDetailed", typeof(string),
                                                                  typeof(int?), typeof(int));
                var method = searchResults.MakeGenericMethod(typeByName);
                return
                    (method.Invoke(_searchService, new object[] { term, parent, page }) as
                     IPagedList<DetailedSearchResultModel>);
            }
            var docs =
                _searchService.SearchDocumentsDetailed<Document>(term, parent, page);

            return docs;
        }

        private List<SearchResultModel> GetResults(string term, string type)
        {
            if (!string.IsNullOrWhiteSpace(type))
            {
                var typeByName = DocumentMetadataHelper.GetTypeByName(type);
                var searchResults = _searchService.GetType().GetMethodExt("SearchDocuments", typeof(string));
                var method = searchResults.MakeGenericMethod(typeByName);
                return (method.Invoke(_searchService, new object[] { term }) as IEnumerable<SearchResultModel>).ToList();
            }
            List<SearchResultModel> docs = _searchService.SearchDocuments<Document>(term).ToList();

            return docs;
        }

        public PartialViewResult GetBreadCrumb(int? parentId)
        {
            //get breadcrumb
            if (parentId.HasValue)
            {
                var allParents = _documentService.GetParents(parentId);

                IEnumerable<NavigationRecord> navigationRecords = allParents.Select(webpage => new NavigationRecord
                    {
                        Text = MvcHtmlString.Create(webpage.Name),
                        Url = MvcHtmlString.Create("/Admin/Search?parent=" + webpage.Id)
                    }).Reverse().ToList();

                return PartialView(new NavigationList(navigationRecords));
            }

            return PartialView(null);
        }
    }
}