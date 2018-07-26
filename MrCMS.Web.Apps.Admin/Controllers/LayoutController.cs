using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Models;
using MrCMS.Web.Apps.Admin.ACL;
using MrCMS.Web.Apps.Admin.Helpers;
using MrCMS.Web.Apps.Admin.Services;
using MrCMS.Website;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Admin.Controllers
{
    public class LayoutController : MrCMSAdminController
    {
        private readonly ILayoutAdminService _layoutAdminService;

        public LayoutController(ILayoutAdminService layoutAdminService)
        {
            _layoutAdminService = layoutAdminService;
        }

        [Acl(typeof(LayoutsACL), LayoutsACL.Show)]
        public ViewResult Index()
        {
            return View();
        }

        [HttpGet]
        [ActionName("Add")]
        [Acl(typeof(LayoutsACL), LayoutsACL.Add)]
        public ViewResult Add_Get(int? id)
        {
            //Build list 
            var model = _layoutAdminService.GetAddLayoutModel(id);

            return View(model);
        }

        [HttpPost]
        public ActionResult Add(Layout doc)
        {
            _layoutAdminService.Add(doc);
            TempData.SuccessMessages().Add(string.Format("{0} successfully added", doc.Name));
            return RedirectToAction("Edit", new {id = doc.Id});
        }

        [HttpGet]
        [ActionName("Edit")]
        [Acl(typeof(LayoutsACL), LayoutsACL.Edit)]
        public ActionResult Edit_Get(Layout doc)
        {
            return View(doc);
        }

        [HttpPost]
        public ActionResult Edit(Layout doc)
        {
            _layoutAdminService.Update(doc);
            TempData.SuccessMessages().Add(string.Format("{0} successfully saved", doc.Name));
            return RedirectToAction("Edit", new {id = doc.Id});
        }

        [HttpGet]
        [ActionName("Delete")]
        [Acl(typeof(LayoutsACL), LayoutsACL.Delete)]
        public ActionResult Delete_Get(Layout document)
        {
            return PartialView(document);
        }

        [HttpPost]
        public ActionResult Delete(Layout document)
        {
            _layoutAdminService.Delete(document);
            TempData.InfoMessages().Add(string.Format("{0} deleted", document.Name));
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Acl(typeof(LayoutsACL), LayoutsACL.Sort)]
        public ActionResult Sort(
            //[IoCModelBinder(typeof(NullableEntityModelBinder))]
            Layout parent) // TODO: model-binding
        {
            var sortItems =
                _layoutAdminService.GetSortItems(parent);

            return View(sortItems);
        }

        [HttpPost]
        public ActionResult Sort(
            //[IoCModelBinder(typeof(NullableEntityModelBinder))]
            Layout parent, // TODO: model-binding
            List<SortItem> items)
        {
            _layoutAdminService.SetOrders(items);
            return RedirectToAction("Sort", parent == null ? null : new {id = parent.Id});
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
            return !_layoutAdminService.UrlIsValidForLayout(urlSegment, id)
                ? Json("Path already in use.")
                : Json(true);
        }

        [HttpGet]
        public PartialViewResult Set(Layout doc)
        {
            ViewData["valid-parents"] = _layoutAdminService.GetValidParents(doc);
            return PartialView();
        }

        [HttpPost]
        public RedirectToActionResult Set(Layout doc, int? parentVal)
        {
            _layoutAdminService.SetParent(doc, parentVal);

            return RedirectToAction("Edit", "Layout", new {id = doc.Id});
        }
    }
}