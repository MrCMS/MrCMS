using System.Web.Mvc;
using MrCMS.Entities.Documents.Layout;
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

        public override ActionResult View(int id)
        {
            var document = _documentService.GetDocument<Layout>(id);

            if (document == null)
                return RedirectToAction("Index");

            return View(document);
        }

        public override ActionResult Add([SessionModelBinder(typeof(AddDocumentModelBinder))]Layout doc)
        {
            _documentService.AddDocument(doc);
            return RedirectToAction("Edit", new { id = doc.Id });
        }
    }
}