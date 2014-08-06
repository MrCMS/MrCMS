using System.Web.Mvc;
using MrCMS.ACL;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Web.Areas.Admin.ModelBinders;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website;
using MrCMS.Website.Binders;
using MrCMS.Website.Filters;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class WebpageController : BaseDocumentController<Webpage>
    {
        private readonly IWebpageBaseViewDataService _webpageBaseViewDataService;

        public WebpageController(IWebpageBaseViewDataService webpageBaseViewDataService,
            IDocumentService documentService, IUrlValidationService urlValidationService)
            : base(documentService, urlValidationService)
        {
            _webpageBaseViewDataService = webpageBaseViewDataService;
        }

        public override ActionResult Add_Get(int? id)
        {
            //Build list 
            var model = new AddPageModel
            {
                Parent = id.HasValue ? _documentService.GetDocument<Webpage>(id.Value) : null
            };
            _webpageBaseViewDataService.SetAddPageViewData(ViewData, model.Parent as Webpage);
            return View(model);
        }

        [ForceImmediateLuceneUpdate]
        public override ActionResult Add([IoCModelBinder(typeof (AddWebpageModelBinder))] Webpage doc)
        {
            if (_urlValidationService.UrlIsValidForWebpage(doc.UrlSegment, null))
                return base.Add(doc);

            _webpageBaseViewDataService.SetAddPageViewData(ViewData, doc.Parent as Webpage);
            return View(doc);
        }

        [MrCMSTypeACL(typeof (Webpage), TypeACLRule.Edit)]
        public override ActionResult Edit_Get(Webpage doc)
        {
            _webpageBaseViewDataService.SetEditPageViewData(ViewData, doc);
            doc.SetAdminViewData(ViewData);
            return base.Edit_Get(doc);
        }

        [MrCMSTypeACL(typeof (Webpage), TypeACLRule.Edit)]
        [ForceImmediateLuceneUpdate]
        public override ActionResult Edit([IoCModelBinder(typeof (EditWebpageModelBinder))] Webpage doc)
        {
            return base.Edit(doc);
        }

        [MrCMSTypeACL(typeof (Webpage), TypeACLRule.Delete)]
        public override ActionResult Delete_Get(Webpage document)
        {
            return base.Delete_Get(document);
        }

        [MrCMSTypeACL(typeof (Webpage), TypeACLRule.Delete)]
        [ForceImmediateLuceneUpdate]
        public override ActionResult Delete(Webpage document)
        {
            return base.Delete(document);
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

            return RedirectToAction("Edit", new {id = webpage.Id});
        }

        [HttpPost]
        public ActionResult Unpublish(Webpage webpage)
        {
            _documentService.Unpublish(webpage);

            return RedirectToAction("Edit", new {id = webpage.Id});
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
        public ActionResult AddProperties([IoCModelBinder(typeof (AddPropertiesModelBinder))] Webpage webpage)
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