using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MrCMS.Entities.People;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class RoleController : Controller
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }
        // GET: /Admin/Role/
        public ActionResult Index()
        {
            return View(_roleService.GetAllRoles());
        }

        [HttpGet]
        public ActionResult Add()
        {
            var model = new UserRole();
            return View(model);
        }

        [HttpPost]
        public ActionResult Add(UserRole model)
        {
            _roleService.SaveRole(model);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            UserRole role = _roleService.GetRole(id);

            if (role == null)
                return RedirectToAction("Index");

            return View(role);
        }

        [HttpPost]
        public ActionResult Edit(UserRole model)
        {
            _roleService.SaveRole(model);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            UserRole role = _roleService.GetRole(id);

            if (role == null)
                return RedirectToAction("Index");

            return View(role);
        }

        [HttpPost]
        public ActionResult Delete(UserRole role)
        {
            _roleService.DeleteRole(role);
            return RedirectToAction("Index");
        }
    }
}
