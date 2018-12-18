using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Models;
using MrCMS.Web.Apps.Admin.Models;
using MrCMS.Web.Apps.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Admin.Controllers
{
    public class LayoutAreaController : MrCMSAdminController
    {
        private readonly ILayoutAreaAdminService _layoutAreaAdminService;

        public LayoutAreaController(ILayoutAreaAdminService layoutAreaAdminService)
        {
            _layoutAreaAdminService = layoutAreaAdminService;
        }

        [HttpGet]
        public PartialViewResult Add(int id)
        {
            return PartialView("Add", _layoutAreaAdminService.GetAddModel(id));
        }

        [HttpPost]
        public RedirectToActionResult Add(AddLayoutAreaModel layoutArea)
        {
            _layoutAreaAdminService.Add(layoutArea);

            return RedirectToAction("Edit", "Layout", new { id = layoutArea.LayoutId });
        }

        [HttpGet]
        [ActionName("Edit")]
        public ActionResult Edit_Get(int id)
        {
            var model = _layoutAreaAdminService.GetEditModel(id);
            if (model == null)
                return RedirectToAction("Index", "Layout");

            ViewData["layout"] = _layoutAreaAdminService.GetLayout(id);
            ViewData["widgets"] = _layoutAreaAdminService.GetWidgets(id);
            return View(model);
        }

        [HttpPost]
        public RedirectToActionResult Edit(UpdateLayoutAreaModel model)
        {
            var area = _layoutAreaAdminService.Update(model);

            return RedirectToAction("Edit", "Layout", new { id = area.Layout.Id });
        }


        [HttpGet]
        [ActionName("Delete")]
        public PartialViewResult Delete_Get(int id)
        {
            return PartialView(_layoutAreaAdminService.GetEditModel(id));
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var layoutArea = _layoutAreaAdminService.DeleteArea(id);

            return RedirectToAction("Edit", "Layout", new { id = layoutArea.Layout.Id });
        }

        [HttpGet]
        public ViewResult SortWidgets(int id, string returnUrl = null)
        {
            var area = _layoutAreaAdminService.GetArea(id);
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
        public ViewResult SortWidgetsForPage(int id, int pageId, string returnUrl = null)
        {
            var area = _layoutAreaAdminService.GetArea(id);
            ViewData["returnUrl"] = returnUrl;
            return View(_layoutAreaAdminService.GetSortModel(area, pageId));
        }

        [HttpPost]
        public ActionResult SortWidgetsForPage(PageWidgetSortModel pageWidgetSortModel, string returnUrl = null)
        {
            _layoutAreaAdminService.SetWidgetForPageOrders(pageWidgetSortModel);

            if (!string.IsNullOrEmpty(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Edit", "Webpage", new { id = pageWidgetSortModel.WebpageId });
        }

        [HttpPost]
        public ActionResult ResetSorting(int id, int pageId, string returnUrl = null)
        {
            _layoutAreaAdminService.ResetSorting(id, pageId);

            if (!string.IsNullOrEmpty(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Edit", "Webpage", new { id = pageId });
        }
    }

}