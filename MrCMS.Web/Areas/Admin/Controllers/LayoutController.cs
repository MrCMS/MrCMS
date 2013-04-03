using System.Web.Mvc;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Media;
using MrCMS.Services;
using MrCMS.Website.Binders;
using NHibernate;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class LayoutController : BaseDocumentController<Layout>
    {
        public LayoutController(IDocumentService documentService)
            : base(documentService)
        {
        }

        [HttpGet]
        [ActionName("Add")]
        public override ActionResult Add_Get(int? id)
        {
            //Build list 
            var model = new Layout()
            {
                Parent = id.HasValue ? _documentService.GetDocument<Layout>(id.Value) : null
            };

            return View(model);
        }

        public override ActionResult Show(Layout document)
        {
            if (document == null)
                return RedirectToAction("Index");

            return View((object)document);
        }

        public override ActionResult Add([IoCModelBinder(typeof(AddDocumentModelBinder))]Layout doc)
        {
            _documentService.AddDocument(doc);
            return RedirectToAction("Edit", new { id = doc.Id });
        }

        /// <summary>
        /// Finds out if the path entered is valid.
        /// </summary>
        /// <param name="UrlSegment">The URL Segment entered</param>
        /// <param name="DocumentType">The type of document</param>
        /// <returns></returns>
        public ActionResult ValidateUrlIsAllowed(string UrlSegment, int? Id)
        {
            return !_documentService.UrlIsValidForLayout(UrlSegment, Id) ? Json("Path already in use.", JsonRequestBehavior.AllowGet) : Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}