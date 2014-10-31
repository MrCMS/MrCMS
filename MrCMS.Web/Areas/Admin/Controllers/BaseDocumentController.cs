using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Documents;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public abstract class BaseDocumentController<T> : MrCMSAdminController where T : Document
    {
        protected readonly IDocumentService _documentService;
        protected readonly IUrlValidationService _urlValidationService;

        protected BaseDocumentController(IDocumentService documentService, IUrlValidationService urlValidationService)
        {
            _documentService = documentService;
            _urlValidationService = urlValidationService;
        }

        public virtual ViewResult Index()
        {
            return View();
        }

        [HttpGet]
        [ActionName("Add")]
        public abstract ActionResult Add_Get(int? id);


        [HttpPost]
        public virtual ActionResult Add(T doc)
        {
            _documentService.AddDocument(doc);
            TempData.SuccessMessages().Add(string.Format("{0} successfully added", doc.Name));
            return RedirectToAction("Edit", new {id = doc.Id});
        }

        [HttpGet]
        [ActionName("Edit")]
        public virtual ActionResult Edit_Get(T doc)
        {
            return View(doc);
        }

        [HttpPost]
        public virtual ActionResult Edit(T doc)
        {
            _documentService.SaveDocument(doc);
            TempData.SuccessMessages().Add(string.Format("{0} successfully saved", doc.Name));
            return RedirectToAction("Edit", new {id = doc.Id});
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
        public ActionResult Sort([IoCModelBinder(typeof (NullableEntityModelBinder))] T parent)
        {
            List<SortItem> sortItems =
                _documentService.GetDocumentsByParent(parent)
                    .Select(
                        arg => new SortItem {Order = arg.DisplayOrder, Id = arg.Id, Name = arg.Name})
                    .OrderBy(x => x.Order)
                    .ToList();

            return View(sortItems);
        }

        [HttpPost]
        public ActionResult Sort([IoCModelBinder(typeof (NullableEntityModelBinder))] T parent, List<SortItem> items)
        {
            _documentService.SetOrders(items);
            return RedirectToAction("Sort", parent == null ? null : new {id = parent.Id});
        }
    }
}