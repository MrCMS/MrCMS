using System.Web.Mvc;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Services;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class LayoutController : BaseDocumentController<Layout>
    {
        public LayoutController(IDocumentService documentService, IUrlValidationService urlValidationService)
            : base(documentService, urlValidationService)
        {
        }

        [HttpGet]
        [ActionName("Add")]
        public override ActionResult Add_Get(int? id)
        {
            //Build list 
            var model = new Layout
            {
                Parent = id.HasValue ? _documentService.GetDocument<Layout>(id.Value) : null
            };

            return View(model);
        }

        public ActionResult Show(Layout document)
        {
            if (document == null)
                return RedirectToAction("Index");

            return View(document);
        }

        public override ActionResult Add(Layout doc)
        {
            _documentService.AddDocument(doc);
            return RedirectToAction("Edit", new {id = doc.Id});
        }

        /// <summary>
        ///     Finds out if the path entered is valid.
        /// </summary>
        /// <param name="urlSegment">The URL Segment entered</param>
        /// <param name="id">The id of the current page (if it exists yet)</param>
        /// <returns></returns>
        public ActionResult ValidateUrlIsAllowed(string urlSegment, int? id)
        {
            return !_urlValidationService.UrlIsValidForLayout(urlSegment, id)
                ? Json("Path already in use.", JsonRequestBehavior.AllowGet)
                : Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}