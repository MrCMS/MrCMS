using Microsoft.AspNetCore.Mvc;
using MrCMS.Models;
using MrCMS.Website;
using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Web.Admin.ACL;
using MrCMS.Web.Admin.Infrastructure.BaseControllers;
using MrCMS.Web.Admin.Infrastructure.Helpers;
using MrCMS.Web.Admin.Models;
using MrCMS.Web.Admin.Services;

namespace MrCMS.Web.Admin.Controllers
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
            TempData.AddSuccessMessage($"{model.Name} successfully added");
            return RedirectToAction("Edit", new {id = layout.Id});
        }

        [HttpGet]
        [ActionName("Edit")]
        [Acl(typeof(LayoutsACL), LayoutsACL.Edit)]
        public async Task<ViewResult> Edit_Get(int id)
        {
            var layout = await _layoutAdminService.GetEditModel(id);
            ViewData["layout-areas"] = await _layoutAdminService.GetLayoutAreas(id);
            return View(layout);
        }

        [HttpPost]
        public async Task<RedirectToActionResult> Edit(UpdateLayoutModel model)
        {
            await _layoutAdminService.Update(model);
            TempData.AddSuccessMessage($"{model.Name} successfully saved");
            return RedirectToAction("Edit", new {id = model.Id});
        }

        [HttpGet]
        [ActionName("Delete")]
        [Acl(typeof(LayoutsACL), LayoutsACL.Delete)]
        public async Task<ActionResult> Delete_Get(int id)
        {
            return PartialView(await _layoutAdminService.GetLayout(id));
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            var layout = await _layoutAdminService.Delete(id);
            TempData.AddInfoMessage($"{layout.Name} deleted");
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
        public async Task<PartialViewResult> Set(int id)
        {
            ViewData["valid-parents"] = await _layoutAdminService.GetValidParents(id);
            return PartialView(id);
        }

        [HttpPost]
        public async Task<RedirectToActionResult> Set(int id, int? parentVal)
        {
            await _layoutAdminService.SetParent(id, parentVal);

            return RedirectToAction("Edit", "Layout", new {id});
        }
    }
}