using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Documents;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;
using MrCMS.Helpers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public abstract class BaseDocumentController<T> : MrCMSAdminController where T : Document
    {
        protected readonly IDocumentService _documentService;

        protected BaseDocumentController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        public ViewResult Index()
        {
            return View();
        }

        [HttpGet]
        [ActionName("Add")]
        public virtual ActionResult Add_Get(int? id)
        {
            //Build list 
            var model = new AddPageModel
            {
                Parent = id.HasValue ? _documentService.GetDocument<Document>(id.Value) : null
            };
            DocumentTypeSetup(model as T);
            return View(model);
        }

        [HttpPost]
        public virtual ActionResult Add([IoCModelBinder(typeof(AddDocumentModelBinder))] T doc)
        {
            _documentService.AddDocument(doc);
            TempData.SuccessMessages().Add(string.Format("{0} successfully added", doc.Name));
            return RedirectToAction("Edit", new { id = doc.Id });
        }

        [HttpGet]
        [ActionName("Edit")]
        public virtual ActionResult Edit_Get(T doc)
        {
            DocumentTypeSetup(doc);
            return View(doc);
        }

        protected virtual void DocumentTypeSetup(T doc)
        {

        }

        [HttpPost]
        public virtual ActionResult Edit([IoCModelBinder(typeof(EditDocumentModelBinder))] T doc)
        {
            _documentService.SaveDocument(doc);
            TempData.SuccessMessages().Add(string.Format("{0} successfully saved", doc.Name));
            return RedirectToAction("Edit", new { id = doc.Id });
        }

        [HttpGet]
        [ActionName("Delete")]
        public virtual ActionResult Delete_Get(T document)
        {
            return PartialView(document);
        }

        [HttpPost]
        public virtual ActionResult Delete(T document)
        {
            _documentService.DeleteDocument(document);
            TempData.InfoMessages().Add(string.Format("{0} deleted", document.Name));
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Sort([IoCModelBinder(typeof(NullableEntityModelBinder))]T parent)
        {
            var sortItems =
                _documentService.GetDocumentsByParent(parent).OrderBy(arg => arg.DisplayOrder)
                                .Select(
                                    arg => new SortItem { Order = arg.DisplayOrder, Id = arg.Id, Name = arg.Name })
                                .ToList();

            return View(sortItems);
        }

        [HttpPost]
        public ActionResult Sort([IoCModelBinder(typeof(NullableEntityModelBinder))]T parent, List<SortItem> items)
        {
            _documentService.SetOrders(items);
            return RedirectToAction("Sort", parent == null ? null : new { id = parent.Id });
        }

        public abstract ActionResult Show(T document);

        
    }
}