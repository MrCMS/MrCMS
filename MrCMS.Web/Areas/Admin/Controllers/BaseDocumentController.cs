using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Documents;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Website.Binders;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public abstract class BaseDocumentController<T> : AdminController where T : Document
    {
        protected readonly IDocumentService _documentService;
        protected readonly ISiteService _siteService;

        protected BaseDocumentController(IDocumentService documentService, ISiteService siteService)
        {
            _documentService = documentService;
            _siteService = siteService;
        }

        //
        // GET: /Admin/Webpage/

        public ViewResult Index()
        {
            return View();
        }

        [HttpGet]
        public virtual ActionResult Add(int? id, int? siteId = null)
        {
            //Build list 
            var model = new AddPageModel
                {
                    Parent = _documentService.GetDocument<Document>(id.GetValueOrDefault(0)),
                    Site = _siteService.GetSite(siteId.GetValueOrDefault())
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
        public ActionResult Edit([SessionModelBinder(typeof(EditDocumentModelBinder))] T doc)
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

        public ActionResult Sort(int? id)
        {
            List<T> categories = _documentService.GetDocumentsByParentId<T>(id).ToList();

            return View(categories);
        }

        public void SortAction(int documentId, int order)
        {
            _documentService.SetOrder(documentId, order);
        }

        public abstract ActionResult Show(T document);
    }
}