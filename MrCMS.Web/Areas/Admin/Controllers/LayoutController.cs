using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.ACL;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class LayoutController : MrCMSAdminController
    {
        private readonly IDocumentService _documentService;
        private readonly IUrlValidationService _urlValidationService;
        private readonly ILayoutAreaAdminService _layoutAreaAdminService;

        public LayoutController(IDocumentService documentService, IUrlValidationService urlValidationService, ILayoutAreaAdminService layoutAreaAdminService)
        {
            _documentService = documentService;
            _urlValidationService = urlValidationService;
            _layoutAreaAdminService = layoutAreaAdminService;
        }

        [MrCMSACLRule(typeof(LayoutsACL), LayoutsACL.Show)]
        public ViewResult Index()
        {
            return View();
        }

        [HttpGet, ActionName("Add"), MrCMSACLRule(typeof(LayoutsACL), LayoutsACL.Add)]
        public ActionResult Add_Get(int? id)
        {
            //Build list 
            var model = new Layout
            {
                Parent = id.HasValue ? _documentService.GetDocument<Layout>(id.Value) : null
            };

            return View(model);
        }

        [HttpPost]
        public virtual ActionResult Add(Layout doc)
        {
            _documentService.AddDocument(doc);
            TempData.SuccessMessages().Add(string.Format("{0} successfully added", doc.Name));
            return RedirectToAction("Edit", new { id = doc.Id });
        }

        [HttpGet, ActionName("Edit"), MrCMSACLRule(typeof(LayoutsACL), LayoutsACL.Edit)]
        public virtual ActionResult Edit_Get(Layout doc)
        {
            return View(doc);
        }

        [HttpPost]
        public virtual ActionResult Edit(Layout doc)
        {
            _documentService.SaveDocument(doc);
            TempData.SuccessMessages().Add(string.Format("{0} successfully saved", doc.Name));
            return RedirectToAction("Edit", new { id = doc.Id });
        }

        [HttpGet, ActionName("Delete"), MrCMSACLRule(typeof(LayoutsACL), LayoutsACL.Delete)]
        public virtual ActionResult Delete_Get(Layout document)
        {
            return PartialView(document);
        }

        [HttpPost]
        public virtual ActionResult Delete(Layout document)
        {
            _documentService.DeleteDocument(document);
            TempData.InfoMessages().Add(string.Format("{0} deleted", document.Name));
            return RedirectToAction("Index");
        }

        [HttpGet, MrCMSACLRule(typeof(LayoutsACL), LayoutsACL.Sort)]
        public ActionResult Sort([IoCModelBinder(typeof(NullableEntityModelBinder))] Layout parent)
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
        public ActionResult Sort([IoCModelBinder(typeof(NullableEntityModelBinder))] Layout parent, List<SortItem> items)
        {
            _documentService.SetOrders(items);
            return RedirectToAction("Sort", parent == null ? null : new { id = parent.Id });
        }

        public ActionResult Show(Layout document)
        {
            if (document == null)
                return RedirectToAction("Index");

            return View(document);
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

        [HttpGet]
        public PartialViewResult Set(Layout doc)
        {
            ViewData["valid-parents"] = _layoutAreaAdminService.GetValidParents(doc);
            return PartialView();
        }

        [HttpPost]
        public RedirectToRouteResult Set(Layout doc, int? parentVal)
        {
            _layoutAreaAdminService.Set(doc, parentVal);

            return RedirectToAction("Edit", "Layout", new { id = doc.Id });
        }
    }
}