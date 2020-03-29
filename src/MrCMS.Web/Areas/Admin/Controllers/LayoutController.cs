using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Models;
using MrCMS.Web.Areas.Admin.ACL;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
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
        public async Task<RedirectToActionResult> Add(AddLayoutModel model)
        {
            var layout = await _layoutAdminService.Add(model);
            TempData.SuccessMessages().Add(string.Format("{0} successfully added", model.Name));
            return RedirectToAction("Edit", new { id = layout.Id });
        }

        [HttpGet]
        [ActionName("Edit")]
        [Acl(typeof(LayoutsACL), LayoutsACL.Edit)]
        public async Task<ViewResult> Edit_Get(int id)
        {
            var layout = _layoutAdminService.GetEditModel(id);
            ViewData["layout-areas"] = await _layoutAdminService.GetLayoutAreas(id);
            return View(layout);
        }

        [HttpPost]
        public async Task<RedirectToActionResult> Edit(UpdateLayoutModel model)
        {
            await _layoutAdminService.Update(model);
            TempData.SuccessMessages().Add(string.Format("{0} successfully saved", model.Name));
            return RedirectToAction("Edit", new { id = model.Id });
        }

        [HttpGet]
        [ActionName("Delete")]
        [Acl(typeof(LayoutsACL), LayoutsACL.Delete)]
        public ActionResult Delete_Get(int id)
        {
            return PartialView(_layoutAdminService.GetLayout(id));
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            var layout =await _layoutAdminService.Delete(id);
            TempData.InfoMessages().Add(string.Format("{0} deleted", layout.Name));
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Acl(typeof(LayoutsACL), LayoutsACL.Sort)]
        public async Task<ViewResult> Sort(int? id)
        {
            var sortItems = await _layoutAdminService.GetSortItems(id);

            return View(sortItems);
        }

        [HttpPost]
        public async Task<ActionResult> Sort(
            int? id,
            List<SortItem> items)
        {
            await _layoutAdminService.SetOrders(items);
            return RedirectToAction("Sort", id);
        }

        /// <summary>
        ///     Finds out if the path entered is valid.
        /// </summary>
        /// <param name="urlSegment">The URL Segment entered</param>
        /// <param name="id">The id of the current page (if it exists yet)</param>
        /// <returns></returns>
        public async Task<ActionResult> ValidateUrlIsAllowed(string urlSegment, int? id)
        {
            return !await _layoutAdminService.UrlIsValidForLayout(urlSegment, id)
                ? Json("Path already in use.")
                : Json(true);
        }

        [HttpGet]
        public PartialViewResult Set(int id)
        {
            ViewData["valid-parents"] = _layoutAdminService.GetValidParents(id);
            return PartialView(id);
        }

        [HttpPost]
        public async Task<RedirectToActionResult> Set(int id, int? parentVal)
        {
            await _layoutAdminService.SetParent(id, parentVal);

            return RedirectToAction("Edit", "Layout", new { id });
        }
    }

}