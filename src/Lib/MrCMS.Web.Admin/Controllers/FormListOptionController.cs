using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Web.Admin.Models;
using MrCMS.Web.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Admin.Controllers
{
    public class FormListOptionController : MrCMSAdminController
    {
        private readonly IFormListOptionAdminService _adminService;

        public FormListOptionController(IFormListOptionAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet]
        public ActionResult Add(int id)
        {
            return View(_adminService.GetAddModel(id));
        }

        [HttpPost]
        public ActionResult Add(AddFormListOptionModel model)
        {
            _adminService.Add(model);
            return Json(new FormActionResult { success = true });
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            return View(_adminService.GetUpdateModel(id));
        }

        [HttpPost]
        [ActionName(nameof(Edit))]
        public ActionResult Edit_POST(UpdateFormListOptionModel model)
        {
            _adminService.Update(model);
            return Json(new FormActionResult { success = true });
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            return View(_adminService.GetUpdateModel(id));
        }

        [HttpPost]
        [ActionName(nameof(Delete))]
        public ActionResult Delete_POST(int id)
        {
            _adminService.Delete(id);
            return Json(new FormActionResult { success = true });
        }

    }
}