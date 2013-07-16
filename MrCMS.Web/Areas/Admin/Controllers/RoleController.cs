using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using MrCMS.ACL.Rules;
using MrCMS.Entities.Documents;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Website;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class RoleController : MrCMSAdminController
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [MrCMSACLRule(typeof(RoleACL),RoleACL.View)]
        public ActionResult Index()
        {
            return View(_roleService.GetAllRoles());
        }

        [HttpGet]
        [MrCMSACLRule(typeof(RoleACL),RoleACL.Add)]
        public PartialViewResult Add()
        {
            var model = new UserRole();
            return PartialView(model);
        }

        [HttpPost]
        [MrCMSACLRule(typeof(RoleACL),RoleACL.Add)]
        public ActionResult Add(UserRole model)
        {
            if (ModelState.IsValid)
            {
                if (_roleService.GetRoleByName(model.Name) == null)
                    _roleService.SaveRole(model);
                else
                    TempData.ErrorMessages().Add(string.Format("{0} already exists.", model.Name));
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        [ActionName("Edit")]
        [MrCMSACLRule(typeof(RoleACL),RoleACL.Edit)]
        public ActionResult Edit_Get(UserRole role)
        {
            if (role == null)
                return RedirectToAction("Index");

            return View(role);
        }

        [HttpPost]
        [MrCMSACLRule(typeof(RoleACL),RoleACL.Edit)]
        public ActionResult Edit(UserRole model)
        {
            _roleService.SaveRole(model);

            return RedirectToAction("Index");
        }

        [HttpGet]
        [ActionName("Delete")]
        [MrCMSACLRule(typeof(RoleACL),RoleACL.Delete)]
        public ActionResult Delete_Get(UserRole role)
        {
            if (role == null)
                return RedirectToAction("Index");

            return View(role);
        }

        [HttpPost]
        [MrCMSACLRule(typeof(RoleACL),RoleACL.Delete)]
        public ActionResult Delete(UserRole role)
        {
            if (role != null) _roleService.DeleteRole(role);
            return RedirectToAction("Index");
        }

        public JsonResult Search(string term)
        {
            var result = _roleService.Search(term);

            return Json(result);
        }

        /// <summary>
        /// Used with Tag-it javascript to act as data source for roles available for securing web pages. See permissions tab in edit view of a webpage.
        /// </summary>
        /// <returns></returns>
        public JsonResult GetRolesForPermissions()
        {
            var roles = _roleService.GetAllRoles().Select(x=>x.Name).ToArray();
            var result = new JavaScriptSerializer().Serialize(roles);
            return Json(result);
        }
    }
}
