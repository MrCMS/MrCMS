using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Entities.Documents;
using MrCMS.Entities.People;
using MrCMS.Models;
using MrCMS.Services;
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
        // GET: /Admin/Role/
        public ActionResult Index()
        {
            return View(_roleService.GetAllRoles());
        }

        [HttpGet]
        public PartialViewResult Add()
        {
            var model = new UserRole();
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult Add(UserRole model)
        {
            _roleService.SaveRole(model);

            return RedirectToAction("Index");
        }

        [HttpGet]
        [ActionName("Edit")]
        public ActionResult Edit_Get(UserRole role)
        {
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
        [ActionName("Delete")]
        public ActionResult Delete_Get(UserRole role)
        {
            if (role == null)
                return RedirectToAction("Index");

            return View(role);
        }

        [HttpPost]
        public ActionResult Delete(UserRole role)
        {
            if (role != null) _roleService.DeleteRole(role);
            return RedirectToAction("Index");
        }

        public JsonResult Search(Document document, string term)
        {
            IEnumerable<AutoCompleteResult> result = _roleService.Search(document, term);

            return Json(result);
        }
    }
}
