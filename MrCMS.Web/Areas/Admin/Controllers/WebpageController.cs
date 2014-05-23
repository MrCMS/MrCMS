using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using MrCMS.ACL;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.ModelBinders;
using MrCMS.Website;
using MrCMS.Website.Binders;
using MrCMS.Website.Filters;
using NHibernate;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class WebpageController : BaseDocumentController<Webpage>
    {
        private readonly IFormService _formService;
        private readonly ISession _session;

        public WebpageController(IDocumentService documentService, IFormService formService, ISession session, Site site)
            : base(documentService, site)
        {
            _formService = formService;
            _session = session;
        }

        [ForceImmediateLuceneUpdate]
        public override ActionResult Add([IoCModelBinder(typeof(AddWebpageModelBinder))] Webpage doc)
        {
            if (_documentService.UrlIsValidForWebpage(doc.UrlSegment, null))
                return base.Add(doc);

            DocumentTypeSetup(doc);
            return View(doc);
        }

        protected override void DocumentTypeSetup(Webpage doc)
        {
            IEnumerable<Layout> layouts =
                _documentService.GetAllDocuments<Layout>().Where(x => x.Hidden == false && x.Site == Site);

            ViewData["Layout"] = layouts.BuildSelectItemList(layout => layout.Name,
                                                             layout => layout.Id.ToString(CultureInfo.InvariantCulture),
                                                             layout =>
                                                             doc != null && doc.Layout != null &&
                                                             doc.Layout.Id == layout.Id,
                                                             SelectListItemHelper.EmptyItem("Default Layout", "0"));

            var documentTypeDefinitions =
                (doc.Parent as Webpage).GetValidWebpageDocumentTypes()
                                       .Where(
                                           metadata =>
                                           CurrentRequestData.CurrentUser.CanAccess<TypeACLRule>(TypeACLRule.Add,
                                                                                             metadata.Type.FullName))
                                       .ToList();
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
        [MrCMSTypeACL(typeof(Webpage), TypeACLRule.Edit)]
        public override ActionResult Edit_Get(Webpage doc)
        {
            return base.Edit_Get(doc);
        }

        [MrCMSTypeACL(typeof(Webpage), TypeACLRule.Edit)]
        [ForceImmediateLuceneUpdate]
        public override ActionResult Edit([IoCModelBinder(typeof(EditWebpageModelBinder))] Webpage doc)
        {
            return base.Edit(doc);
        }

        [MrCMSTypeACL(typeof(Webpage), TypeACLRule.Delete)]
        public override ActionResult Delete_Get(Webpage document)
        {
            return base.Delete_Get(document);
        }

        [MrCMSTypeACL(typeof(Webpage), TypeACLRule.Delete)]
        [ForceImmediateLuceneUpdate]
        public override ActionResult Delete(Webpage document)
        {
            return base.Delete(document);
        }

        public ActionResult Show(Webpage document)
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

        [HttpGet]
        public PartialViewResult SetParent(Webpage webpage)
        {
            ViewData["valid-parents"] = _documentService.GetValidParents(webpage);
            return PartialView(webpage);
        }

        [HttpPost]
        public RedirectToRouteResult SetParent(Webpage webpage, int? parentVal)
        {
            _documentService.SetParent(webpage, parentVal);

            return RedirectToAction("Edit", new { id = webpage.Id });
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
        /// <summary>
        /// Returns server date used for publishing (can't use JS date as can be out compared to server date)
        /// </summary>
        /// <returns>Date</returns>
        public string GetServerDate()
        {
            return CurrentRequestData.Now.ToString(CurrentRequestData.CultureInfo);
        }

        [HttpGet]
        public PartialViewResult RevertToVersion(DocumentVersion documentVersion)
        {
            return PartialView(documentVersion);
        }

        [HttpPost]
        [ActionName("RevertToVersion")]
        public RedirectToRouteResult RevertToVersion_POST(DocumentVersion documentVersion)
        {
            _documentService.RevertToVersion(documentVersion);
            return RedirectToAction("Edit", new { id = documentVersion.Document.Id });
        }

        [HttpGet]
        public ActionResult AddProperties([IoCModelBinder(typeof(AddPropertiesModelBinder))] Webpage webpage)
        {
            if (webpage != null)
            {
                webpage.AdminViewData(ViewData, _session);
                return PartialView(webpage);
            }
            return new EmptyResult();
        }
    }
}