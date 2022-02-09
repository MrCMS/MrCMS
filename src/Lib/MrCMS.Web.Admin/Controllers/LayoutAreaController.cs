using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Models;
using MrCMS.Web.Admin.Infrastructure.BaseControllers;
using MrCMS.Web.Admin.Models;
using MrCMS.Web.Admin.Services;

namespace MrCMS.Web.Admin.Controllers
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

            return RedirectToAction("Edit", "Layout", new {id = layoutArea.LayoutId});
        }

        [HttpGet]
        [ActionName("Edit")]
        public async Task<ActionResult> Edit_Get(int id)
        {
            var model = await _layoutAreaAdminService.GetEditModel(id);
            if (model == null)
                return RedirectToAction("Index", "Layout");

            ViewData["layout"] = await _layoutAreaAdminService.GetLayout(id);
            ViewData["widgets"] = await _layoutAreaAdminService.GetWidgets(id);
            return View(model);
        }

        [HttpPost]
        public async Task<RedirectToActionResult> Edit(UpdateLayoutAreaModel model)
        {
            var area = await _layoutAreaAdminService.Update(model);

            return RedirectToAction("Edit", "Layout", new {id = area.Layout.Id});
        }


        [HttpGet]
        [ActionName("Delete")]
        public async Task<PartialViewResult> Delete_Get(int id)
        {
            return PartialView(await _layoutAreaAdminService.GetEditModel(id));
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            var layoutArea = await _layoutAreaAdminService.DeleteArea(id);

            return RedirectToAction("Edit", "Layout", new {id = layoutArea.Layout.Id});
        }

        [HttpGet]
        public async Task<ViewResult> SortWidgets(int id, string returnUrl = null)
        {
            var area = await _layoutAreaAdminService.GetArea(id);
            ViewData["returnUrl"] = returnUrl;
            return View(new WidgetSortModel(await _layoutAreaAdminService.GetWidgets(id), area));
        }

        [HttpPost]
        public async Task<RedirectResult> SortWidgetsAction(WidgetSortModel widgetSortModel, string returnUrl = null)
        {
            await _layoutAreaAdminService.SetWidgetOrders(widgetSortModel);

            return Redirect(!string.IsNullOrEmpty(returnUrl)
                ? returnUrl
                : "/Admin/LayoutArea/Edit/" + widgetSortModel.LayoutAreaId);
        }
    }
}