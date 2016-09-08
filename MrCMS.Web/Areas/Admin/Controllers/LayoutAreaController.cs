using System.Web.Mvc;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Models;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class LayoutAreaController : MrCMSAdminController
    {
        private readonly ILayoutAreaAdminService _layoutAreaAdminService;

        public LayoutAreaController(ILayoutAreaAdminService layoutAreaAdminService)
        {
            _layoutAreaAdminService = layoutAreaAdminService;
        }

        [HttpGet]
        public PartialViewResult Add(Layout layout)
        {
            return PartialView("Add", new LayoutArea {Layout = layout});
        }

        [HttpPost]
        public ActionResult Add(LayoutArea layoutArea)
        {
            _layoutAreaAdminService.Add(layoutArea);

            return RedirectToAction("Edit", "Layout", new {id = layoutArea.Layout.Id});
        }

        [HttpGet]
        [ActionName("Edit")]
        public ActionResult Edit_Get(LayoutArea layoutArea)
        {
            if (layoutArea == null)
                return RedirectToAction("Index", "Layout");

            return View(layoutArea);
        }

        [HttpPost]
        public ActionResult Edit(LayoutArea area)
        {
            _layoutAreaAdminService.Update(area);

            return RedirectToAction("Edit", "Layout", new {id = area.Layout.Id});
        }


        [HttpGet]
        [ActionName("Delete")]
        public ActionResult Delete_Get(LayoutArea area)
        {
            return PartialView(area);
        }

        [HttpPost]
        public ActionResult Delete(LayoutArea area)
        {
            int layoutId = area.Layout.Id;
            _layoutAreaAdminService.DeleteArea(area);

            return RedirectToAction("Edit", "Layout", new {id = layoutId});
        }

        [HttpGet]
        public ViewResult SortWidgets(LayoutArea area, string returnUrl = null)
        {
            ViewData["returnUrl"] = returnUrl;
            return View(new PageWidgetSortModel(area.GetWidgets(), area, null));
        }

        [HttpPost]
        public RedirectResult SortWidgetsAction(PageWidgetSortModel pageWidgetSortModel, string returnUrl = null)
        {
            _layoutAreaAdminService.SetWidgetOrders(pageWidgetSortModel);

            return Redirect(!string.IsNullOrEmpty(returnUrl)
                ? returnUrl
                : "/Admin/LayoutArea/Edit/" + pageWidgetSortModel.LayoutAreaId);
        }

        [HttpGet]
        public ViewResult SortWidgetsForPage(LayoutArea area, int pageId, string returnUrl = null)
        {
            ViewData["returnUrl"] = returnUrl;
            return View(_layoutAreaAdminService.GetSortModel(area, pageId));
        }

        [HttpPost]
        public ActionResult SortWidgetsForPage(PageWidgetSortModel pageWidgetSortModel, string returnUrl = null)
        {
            _layoutAreaAdminService.SetWidgetForPageOrders(pageWidgetSortModel);

            if (!string.IsNullOrEmpty(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Edit", "Webpage", new {id = pageWidgetSortModel.WebpageId});
        }

        [HttpPost]
        public ActionResult ResetSorting(LayoutArea area, int pageId, string returnUrl = null)
        {
            _layoutAreaAdminService.ResetSorting(area, pageId);

            if (!string.IsNullOrEmpty(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Edit", "Webpage", new {id = pageId});
        }
    }
}