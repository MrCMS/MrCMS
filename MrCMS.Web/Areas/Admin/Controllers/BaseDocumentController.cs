using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Documents;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Website.Binders;
using NHibernate;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public abstract class BaseDocumentController<T> : AdminController where T : Document
    {
        protected readonly IDocumentService _documentService;

        protected BaseDocumentController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        //
        // GET: /Admin/Webpage/

        public ViewResult Index()
        {
            return View();
        }

        [HttpGet]
        public virtual ActionResult Add(int? id)
        {
            //Build list 
            var model = new AddPageModel
                {
                    Parent = _documentService.GetDocument<Document>(id.GetValueOrDefault(0))
                };
            PopulateEditDropdownLists(model as T);
            return View(model);
        }

        [HttpPost]
        public virtual ActionResult Add([SessionModelBinder(typeof(AddDocumentModelBinder))] T doc)
        {
            _documentService.AddDocument(doc);
            return RedirectToAction("Edit", new { id = doc.Id });
        }

        public string SuggestDocumentUrl(int? parentId, string pageName)
        {
            return _documentService.GetDocumentUrl(pageName, parentId);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var doc = _documentService.GetDocument<T>(id);
            PopulateEditDropdownLists(doc);
            return View(doc);
        }

        protected virtual void PopulateEditDropdownLists(T doc)
        {
        }

        [HttpPost]
        public ActionResult Edit([SessionModelBinder(typeof(EditDocumentModelBinder))] T doc)
        {
            _documentService.SaveDocument(doc);
            TempData["saved"] = string.Format("{0} successfully saved", doc.Name);
            return RedirectToAction("Edit", new { id = doc.Id });
        }

        [HttpGet]
        [ActionName("Delete")]
        public ActionResult Delete_Get(int id)
        {
            var doc = _documentService.GetDocument<T>(id);
            return PartialView(doc);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            _documentService.DeleteDocument<T>(id);

            return RedirectToAction("Index");
        }

        public ActionResult Sort(int? id)
        {
            List<T> categories = _documentService.GetAdminDocumentsByParentId<T>(id).ToList();

            return View(categories);
        }

        public void SortAction(int documentId, int order)
        {
            _documentService.SetOrder(documentId, order);
        }

        public abstract ActionResult View(int id);
    }
}