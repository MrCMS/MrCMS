using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.ACL.Rules;
using MrCMS.Entities.People;
using MrCMS.Models;
using MrCMS.Web.Apps.Admin.Helpers;
using MrCMS.Web.Apps.Admin.Models;
using MrCMS.Web.Apps.Admin.Services;
using MrCMS.Website;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Admin.Controllers
{
    public class RoleController : MrCMSAdminController
    {
        private readonly IRoleAdminService _roleAdminService;

        public RoleController(IRoleAdminService roleAdminService)
        {
            _roleAdminService = roleAdminService;
        }

        [Acl(typeof(RoleACL), RoleACL.View)]
        public ActionResult Index()
        {
            return View(_roleAdminService.GetAllRoles());
        }

        [HttpGet]
        [Acl(typeof(RoleACL), RoleACL.Add)]
        public PartialViewResult Add()
        {
            var model = new Role();
            return PartialView(model);
        }

        [HttpPost]
        [Acl(typeof(RoleACL), RoleACL.Add)]
        public async Task<RedirectToActionResult> Add(AddRoleModel model)
        {
            var addRoleResult = await _roleAdminService.AddRole(model);
            if (!addRoleResult.Success)
                TempData.ErrorMessages().Add(addRoleResult.Error);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [ActionName("Edit")]
        [Acl(typeof(RoleACL), RoleACL.Edit)]
        public ActionResult Edit_Get(int id)
        {
            var role = _roleAdminService.GetEditModel(id);
            if (role == null)
                return RedirectToAction("Index");

            return View(role);
        }

        [HttpPost]
        [Acl(typeof(RoleACL), RoleACL.Edit)]
        public async Task<RedirectToActionResult> Edit(UpdateRoleModel model)
        {
            await _roleAdminService.SaveRole(model);

            return RedirectToAction("Index");
        }

        [HttpGet]
        [ActionName("Delete")]
        [Acl(typeof(RoleACL), RoleACL.Delete)]
        public ActionResult Delete_Get(int id)
        {
            var role = _roleAdminService.GetEditModel(id);
            if (role == null)
                return RedirectToAction("Index");

            return View(role);
        }

        [HttpPost]
        [Acl(typeof(RoleACL), RoleACL.Delete)]
        public async Task<RedirectToActionResult> Delete(int id)
        {
            await _roleAdminService.DeleteRole(id);
            return RedirectToAction("Index");
        }

        public JsonResult Search(string term)
        {
            IEnumerable<AutoCompleteResult> result = _roleAdminService.Search(term);

            return Json(result);
        }

        /// <summary>
        ///     Used with Tag-it javascript to act as data source for roles available for securing web pages. See permissions tab
        ///     in edit view of a webpage.
        /// </summary>
        /// <returns></returns>
        public JsonResult GetRolesForPermissions()
        {
            return Json(_roleAdminService.GetRolesForPermissions());
        }
    }
}