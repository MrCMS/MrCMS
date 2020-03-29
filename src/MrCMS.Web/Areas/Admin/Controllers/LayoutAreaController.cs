using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Models;
using MrCMS.Web.Areas.Admin.Models;
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
        public PartialViewResult Add(int id)
        {
            return PartialView("Add", _layoutAreaAdminService.GetAddModel(id));
        }

        [HttpPost]
        public async Task<RedirectToActionResult> Add(AddLayoutAreaModel layoutArea)
        {
            await _layoutAreaAdminService.Add(layoutArea);

            return RedirectToAction("Edit", "Layout", new { id = layoutArea.LayoutId });
        }

        [HttpGet]
        [ActionName("Edit")]
        public async Task<ActionResult> Edit_Get(int id)
        {
            var model = _layoutAreaAdminService.GetEditModel(id);
            if (model == null)
                return RedirectToAction("Index", "Layout");

            ViewData["layout"] = _layoutAreaAdminService.GetLayout(id);
            ViewData["widgets"] = await _layoutAreaAdminService.GetWidgets(id);
            return View(model);
        }

        [HttpPost]
        public async Task<RedirectToActionResult> Edit(UpdateLayoutAreaModel model)
        {
            var area = await _layoutAreaAdminService.Update(model);

            return RedirectToAction("Edit", "Layout", new { id = area.Layout.Id });
        }


        [HttpGet]
        [ActionName("Delete")]
        public PartialViewResult Delete_Get(int id)
        {
            return PartialView(_layoutAreaAdminService.GetEditModel(id));
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            var layoutArea = await _layoutAreaAdminService.DeleteArea(id);

            return RedirectToAction("Edit", "Layout", new { id = layoutArea.LayoutId });
        }

        [HttpGet]
        public async Task<ViewResult> SortWidgets(int id, string returnUrl = null)
        {
            var area = _layoutAreaAdminService.GetArea(id);
            ViewData["returnUrl"] = returnUrl;
            return View(await _layoutAreaAdminService.GetSortModel(area, null));
        }

        [HttpPost]
        public async Task<RedirectResult> SortWidgetsAction(PageWidgetSortModel pageWidgetSortModel, string returnUrl = null)
        {
            await _layoutAreaAdminService.SetWidgetOrders(pageWidgetSortModel);

            return Redirect(!string.IsNullOrEmpty(returnUrl)
                ? returnUrl
                : "/Admin/LayoutArea/Edit/" + pageWidgetSortModel.LayoutAreaId);
        }

        [HttpGet]
        public async Task<ViewResult> SortWidgetsForPage(int id, int pageId, string returnUrl = null)
        {
            var area = _layoutAreaAdminService.GetArea(id);
            ViewData["returnUrl"] = returnUrl;
            return View(await _layoutAreaAdminService.GetSortModel(area, pageId));
        }

        [HttpPost]
        public async Task<ActionResult> SortWidgetsForPage(PageWidgetSortModel pageWidgetSortModel, string returnUrl = null)
        {
            await _layoutAreaAdminService.SetWidgetForPageOrders(pageWidgetSortModel);

            if (!string.IsNullOrEmpty(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Edit", "Webpage", new { id = pageWidgetSortModel.WebpageId });
        }

        [HttpPost]
        public async Task<ActionResult> ResetSorting(int id, int pageId, string returnUrl = null)
        {
            await _layoutAreaAdminService.ResetSorting(id, pageId);

            if (!string.IsNullOrEmpty(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Edit", "Webpage", new { id = pageId });
        }
    }

}