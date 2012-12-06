using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Documents;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Paging;
using MrCMS.Services;
using NHibernate;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class SearchController : AdminController
    {
        private readonly IDocumentService _documentService;
        private readonly INavigationService _navigationService;
        private readonly ISitesService _sitesService;

        public SearchController(IDocumentService documentService, INavigationService navigationService, ISitesService sitesService)
        {
            _documentService = documentService;
            _navigationService = navigationService;
            _sitesService = sitesService;
        }

        [HttpGet]
        public ActionResult Index(string term, string type, int? parent, int? siteId, int page = 1)
        {
            ViewData["term"] = term;
            ViewData["siteId"] = siteId;
            ViewData["parent-val"] = parent;
            ViewData["parents"] = _navigationService.GetParentsList(_sitesService.GetCurrentSite());
            ViewData["type"] = type;
            ViewData["doc-types"] = _navigationService.GetDocumentTypes(type);

            var docs = GetDetailedResults(term, type, parent, page);
            return View(docs);
        }

        [HttpPost]
        [ActionName("Index")]
        public ActionResult IndexPost(string term, string type, int? parent, int? siteId)
        {
            return RedirectToAction("Index", new {term, type, parent, siteId});
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
                var typeByName = DocumentTypeHelper.GetTypeByName(type);
                var searchResults = _documentService.GetType()
                                                    .GetMethodExt("SearchDocumentsDetailed", typeof(string),
                                                                  typeof(int?), typeof(int));
                var method = searchResults.MakeGenericMethod(typeByName);
                return
                    (method.Invoke(_documentService, new object[] { term, parent, page }) as
                     IPagedList<DetailedSearchResultModel>);
            }
            var docs =
                _documentService.SearchDocumentsDetailed<Document>(term, parent, page);

            return docs;
        }

        private List<SearchResultModel> GetResults(string term, string type)
        {
            if (!string.IsNullOrWhiteSpace(type))
            {
                var typeByName = DocumentTypeHelper.GetTypeByName(type);
                var searchResults = _documentService.GetType().GetMethodExt("SearchDocuments", typeof(string));
                var method = searchResults.MakeGenericMethod(typeByName);
                return (method.Invoke(_documentService, new object[] { term }) as IEnumerable<SearchResultModel>).ToList();
            }
            List<SearchResultModel> docs = _documentService.SearchDocuments<Document>(term).ToList();

            return docs;
        }
    }
}