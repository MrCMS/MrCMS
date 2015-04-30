using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.ACL;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Web.Areas.Admin.ModelBinders;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;
using MrCMS.Website.Filters;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class WebpageController : MrCMSAdminController
    {
        private readonly IWebpageBaseViewDataService _webpageBaseViewDataService;
        private readonly IDocumentService _documentService;
        private readonly IUrlValidationService _urlValidationService;

        public WebpageController(IWebpageBaseViewDataService webpageBaseViewDataService,
            IDocumentService documentService, IUrlValidationService urlValidationService)
        {
            _webpageBaseViewDataService = webpageBaseViewDataService;
            _documentService = documentService;
            _urlValidationService = urlValidationService;
        }

        public ViewResult Index()
        {
            return View();
        }


        [HttpGet, ActionName("Add")]
        public ActionResult Add_Get(int? id)
        {
            //Build list 
            var model = new AddPageModel
            {
                Parent = id.HasValue ? _documentService.GetDocument<Webpage>(id.Value) : null
            };
            _webpageBaseViewDataService.SetAddPageViewData(ViewData, model.Parent as Webpage);
            return View(model);
        }

        [HttpPost, ForceImmediateLuceneUpdate]
        public virtual ActionResult Add([IoCModelBinder(typeof(AddWebpageModelBinder))] Webpage doc)
        {
            if (!_urlValidationService.UrlIsValidForWebpage(doc.UrlSegment, null))
            {
                _webpageBaseViewDataService.SetAddPageViewData(ViewData, doc.Parent as Webpage);
                return View(doc);
            }
            _documentService.AddDocument(doc);
            TempData.SuccessMessages().Add(string.Format("{0} successfully added", doc.Name));
            return RedirectToAction("Edit", new { id = doc.Id });
        }

        [HttpGet, ActionName("Edit"), MrCMSTypeACL(typeof(Webpage), TypeACLRule.Edit)]
        public virtual ActionResult Edit_Get(Webpage doc)
        {
            _webpageBaseViewDataService.SetEditPageViewData(ViewData, doc);
            doc.SetAdminViewData(ViewData);
            return View(doc);
        }

        [HttpPost, MrCMSTypeACL(typeof(Webpage), TypeACLRule.Edit), ForceImmediateLuceneUpdate]
        public virtual ActionResult Edit([IoCModelBinder(typeof(EditWebpageModelBinder))] Webpage doc)
        {
            _documentService.SaveDocument(doc);
            TempData.SuccessMessages().Add(string.Format("{0} successfully saved", doc.Name));
            return RedirectToAction("Edit", new { id = doc.Id });
        }

        [HttpGet, MrCMSTypeACL(typeof(Webpage), TypeACLRule.Delete), ActionName("Delete")]
        public virtual ActionResult Delete_Get(Webpage document)
        {
            return PartialView(document);
        }

        [HttpPost, MrCMSTypeACL(typeof(Webpage), TypeACLRule.Delete), ForceImmediateLuceneUpdate]
        public virtual ActionResult Delete(Webpage document)
        {
            _documentService.DeleteDocument(document);
            TempData.InfoMessages().Add(string.Format("{0} deleted", document.Name));
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Sort([IoCModelBinder(typeof(NullableEntityModelBinder))] Webpage parent)
        {
            List<SortItem> sortItems =
                _documentService.GetDocumentsByParent(parent)
                    .Select(
                        arg => new SortItem { Order = arg.DisplayOrder, Id = arg.Id, Name = arg.Name })
                    .OrderBy(x => x.Order)
                    .ToList();

            return View(sortItems);
        }

        [HttpPost]
        public ActionResult Sort([IoCModelBinder(typeof(NullableEntityModelBinder))] Webpage parent, List<SortItem> items)
        {
            _documentService.SetOrders(items);
            return RedirectToAction("Sort", parent == null ? null : new { id = parent.Id });
        }
        public ActionResult Show(Webpage document)
        {
            if (document == null)
                return RedirectToAction("Index");

            return View(document);
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

        public ActionResult ViewChanges(DocumentVersion documentVersion)
        {
            if (documentVersion == null)
                return RedirectToAction("Index");

            return PartialView(documentVersion);
        }

        [HttpGet]
        public PartialViewResult FormProperties(Webpage webpage)
        {
            return PartialView(webpage);
        }

        /// <summary>
        ///     Finds out if the URL entered is valid for a webpage
        /// </summary>
        /// <param name="urlSegment">The URL Segment entered</param>
        /// <param name="id">The Id of the current document if it is set</param>
        /// <returns></returns>
        public ActionResult ValidateUrlIsAllowed(string urlSegment, int? id)
        {
            return !_urlValidationService.UrlIsValidForWebpage(urlSegment, id)
                ? Json("Please choose a different URL as this one is already used.", JsonRequestBehavior.AllowGet)
                : Json(true, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///     Returns server date used for publishing (can't use JS date as can be out compared to server date)
        /// </summary>
        /// <returns>Date</returns>
        public string GetServerDate()
        {
            return CurrentRequestData.Now.ToString(CurrentRequestData.CultureInfo);
        }

        [HttpGet]
        public ActionResult AddProperties([IoCModelBinder(typeof(AddPropertiesModelBinder))] Webpage webpage)
        {
            if (webpage != null)
            {
                webpage.SetAdminViewData(ViewData);
                return PartialView(webpage);
            }
            return new EmptyResult();
        }
    }
}