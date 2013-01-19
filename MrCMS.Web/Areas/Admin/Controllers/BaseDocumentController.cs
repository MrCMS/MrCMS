using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Multisite;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public abstract class BaseDocumentController<T> : AdminController where T : Document
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
        public virtual ActionResult Add_Get(T parent)
        {
            //Build list 
            var model = new AddPageModel
            {
                Parent = parent
            };
            PopulateEditDropdownLists(model as T);
            return View(model);
        }

        [HttpPost]
        public virtual ActionResult Add([IoCModelBinder(typeof(AddDocumentModelBinder))] T doc)
        {
            _documentService.AddDocument(doc);
            return RedirectToAction("Edit", new { id = doc.Id });
        }

        [HttpGet]
        [ActionName("Edit")]
        public ActionResult Edit_Get(T doc)
        {
            PopulateEditDropdownLists(doc);
            return View(doc);
        }

        protected virtual void PopulateEditDropdownLists(T doc)
        {
        }

        [HttpPost]
        public ActionResult Edit([IoCModelBinder(typeof(EditDocumentModelBinder))] T doc)
        {
            _documentService.SaveDocument(doc);
            TempData["saved"] = string.Format("{0} successfully saved", doc.Name);
            return RedirectToAction("Edit", new { id = doc.Id });
        }

        [HttpGet]
        [ActionName("Delete")]
        public ActionResult Delete_Get(T document)
        {
            return PartialView(document);
        }

        [HttpPost]
        public ActionResult Delete(T document)
        {
            _documentService.DeleteDocument(document);

            return RedirectToAction("Index");
        }

        public ActionResult Sort(T parent)
        {
            List<T> categories = _documentService.GetDocumentsByParent(parent).ToList();

            return View(categories);
        }

        public void SortAction(int documentId, int order)
        {
            _documentService.SetOrder(documentId, order);
        }

        public abstract ActionResult Show(T document);
    }
}