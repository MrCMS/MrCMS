using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using MrCMS.ACL.Rules;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class RoleController : MrCMSAdminController
    {
        private readonly IRoleAdminService _roleAdminService;

        public RoleController(IRoleAdminService roleAdminService)
        {
            _roleAdminService = roleAdminService;
        }

        [MrCMSACLRule(typeof(RoleACL), RoleACL.View)]
        public ActionResult Index()
        {
            return View(_roleAdminService.GetAllRoles());
        }

        [HttpGet]
        [MrCMSACLRule(typeof(RoleACL), RoleACL.Add)]
        public PartialViewResult Add()
        {
            var model = new UserRole();
            return PartialView(model);
        }

        [HttpPost]
        [MrCMSACLRule(typeof(RoleACL), RoleACL.Add)]
        public ActionResult Add(UserRole model)
        {
            var addRoleResult = _roleAdminService.AddRole(model);
            if (!addRoleResult.Success)
                TempData.ErrorMessages().Add(addRoleResult.Error);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [ActionName("Edit")]
        [MrCMSACLRule(typeof(RoleACL), RoleACL.Edit)]
        public ActionResult Edit_Get(UserRole role)
        {
            if (role == null)
                return RedirectToAction("Index");

            return View(role);
        }

        [HttpPost]
        [MrCMSACLRule(typeof(RoleACL), RoleACL.Edit)]
        public ActionResult Edit(UserRole model)
        {
            _roleAdminService.SaveRole(model);

            return RedirectToAction("Index");
        }

        [HttpGet]
        [ActionName("Delete")]
        [MrCMSACLRule(typeof(RoleACL), RoleACL.Delete)]
        public ActionResult Delete_Get(UserRole role)
        {
            if (role == null)
                return RedirectToAction("Index");

            return View(role);
        }

        [HttpPost]
        [MrCMSACLRule(typeof(RoleACL), RoleACL.Delete)]
        public ActionResult Delete(UserRole role)
        {
            if (role != null) _roleAdminService.DeleteRole(role);
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