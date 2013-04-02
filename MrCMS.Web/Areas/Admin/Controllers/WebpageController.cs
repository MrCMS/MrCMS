using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Website;
using MrCMS.Website.Binders;
using NHibernate;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class WebpageController : BaseDocumentController<Webpage>
    {
        private readonly IFormService _formService;
        private readonly ISession _session;

        public WebpageController(IDocumentService documentService, IFormService formService, ISession session)
            : base(documentService)
        {
            _formService = formService;
            _session = session;
        }

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            var id = filterContext.RouteData.Values["id"];
            int idVal;
            if (!string.IsNullOrWhiteSpace(Convert.ToString(id)) && int.TryParse(Convert.ToString(id), out idVal))
            {
                var document = _documentService.GetDocument<Webpage>(idVal);
                if (document != null && !document.IsAllowedForAdmin(CurrentRequestData.CurrentUser))
                {
                    TempData.ErrorMessages().Add("You are not allowed to view that page");
                    filterContext.Result = new RedirectResult("~/admin");
                }
            }
        }

        public override ActionResult Add([IoCModelBinder(typeof(AddDocumentModelBinder))] Webpage doc)
        {
            if (_documentService.UrlIsValidForWebpage(doc.UrlSegment, null))
                return base.Add(doc);

            DocumentTypeSetup(doc);
            return View(doc);
        }

        protected override void DocumentTypeSetup(Webpage doc)
        {
            IEnumerable<Layout> layouts =
                _documentService.GetAllDocuments<Layout>().Where(x => x.Hidden == false && x.Site == CurrentSite);

            ViewData["Layout"] = layouts.BuildSelectItemList(layout => layout.Name,
                                                             layout => layout.Id.ToString(CultureInfo.InvariantCulture),
                                                             layout =>
                                                             doc != null && doc.Layout != null &&
                                                             doc.Layout.Id == layout.Id,
                                                             SelectListItemHelper.EmptyItem("Default Layout"));

            var documentTypeDefinitions =
                (doc.Parent as Webpage).GetValidWebpageDocumentTypes(_documentService, CurrentSite).ToList();
            ViewData["DocumentTypes"] = documentTypeDefinitions;

            doc.AdminViewData(ViewData, _session);

            var documentMetadata = doc.GetMetadata();
            if (documentMetadata != null)
            {
                ViewData["EditView"] = documentMetadata.EditPartialView;
                if (!string.IsNullOrWhiteSpace(documentMetadata.App))
                    RouteData.DataTokens["app"] = documentMetadata.App;
            }
        }

        public override ActionResult Show(Webpage document)
        {
            if (document == null)
                return RedirectToAction("Index");

            return View((object)document);
        }

        [HttpPost]
        public ActionResult PublishNow(Webpage webpage)
        {
            _documentService.PublishNow(webpage);

            return RedirectToAction("Edit", new { id = webpage.Id });
        }

        [HttpPost]
        public ActionResult Unpublish(Webpage webpage)
        {
            _documentService.Unpublish(webpage);

            return RedirectToAction("Edit", new { id = webpage.Id });
        }
        [HttpPost]
        public ActionResult HideWidget(Webpage document, int widgetId, int layoutAreaId)
        {
            _documentService.HideWidget(document, widgetId);
            return RedirectToAction("Edit", new { id = document.Id, layoutAreaId });
        }

        [HttpPost]
        public ActionResult ShowWidget(Webpage document, int widgetId, int layoutAreaId)
        {
            _documentService.ShowWidget(document, widgetId);
            return RedirectToAction("Edit", new { id = document.Id, layoutAreaId });
        }

        [HttpPost]
        public void SetParent(Webpage webpage, int? parentId)
        {
            _documentService.SetParent(webpage, parentId);
        }

        public ActionResult ViewChanges(DocumentVersion documentVersion)
        {
            if (documentVersion == null)
                return RedirectToAction("Index");

            return PartialView(documentVersion);
        }

        public PartialViewResult Postings(Webpage webpage, int page = 1, string search = null)
        {
            var data = _formService.GetFormPostings(webpage, page, search);

            return PartialView(data);
        }

        public ActionResult ViewPosting(FormPosting formPosting)
        {
            return PartialView(formPosting);
        }

        public ActionResult Versions(Document doc, int page = 1)
        {
            var data = doc.GetVersions(page);

            return PartialView(data);
        }

        public string SuggestDocumentUrl(Webpage parent, string pageName)
        {
            return _documentService.GetDocumentUrl(pageName, parent, true);
        }

        [HttpGet]
        public PartialViewResult FormProperties(Webpage webpage)
        {
            return PartialView(webpage);
        }

        /// <summary>
        /// Finds out if the URL entered is valid for a webpage
        /// </summary>
        /// <param name="UrlSegment">The URL Segment entered</param>
        /// <param name="DocumentType">The type of document</param>
        /// <returns></returns>
        public ActionResult ValidateUrlIsAllowed(string UrlSegment, int? Id)
        {
            return !_documentService.UrlIsValidForWebpage(UrlSegment, Id) ? Json("Please choose a different URL as this one is already used.", JsonRequestBehavior.AllowGet) : Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}