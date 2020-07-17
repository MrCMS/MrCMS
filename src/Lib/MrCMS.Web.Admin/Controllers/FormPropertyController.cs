using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Admin.Models;
using MrCMS.Web.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Admin.Controllers
{
    public class FormPropertyController : MrCMSAdminController
    {
        private readonly IFormPropertyAdminService _adminService;

        public FormPropertyController(IFormPropertyAdminService adminService)
        {
            _adminService = adminService;
        }
        [HttpGet]
        public ViewResult Add(int id)
        {
            ViewData["property-types"] = _adminService.GetPropertyTypeOptions();
            return View(new AddFormPropertyModel { FormId = id });
        }

        [HttpPost]
        public JsonResult Add(AddFormPropertyModel model)
        {
            _adminService.Add(model);
            return Json(new FormActionResult { success = true });
        }

        [HttpGet]
        public ViewResult Edit(int id)
        {
            return View(_adminService.GetUpdateModel(id));
        }

        [HttpPost]
        [ActionName(nameof(Edit))]
        public JsonResult Edit_POST(UpdateFormPropertyModel model)
        {
            _adminService.Update(model);
            return Json(new FormActionResult { success = true });
        }

        [HttpGet]
        public ViewResult Delete(int id)
        {
            return View(_adminService.GetUpdateModel(id));
        }
        [HttpPost]
        [ActionName(nameof(Delete))]
        public JsonResult Delete_POST(int id)
        {
            _adminService.Delete(id);
            return Json(new FormActionResult { success = true });
        }

    }
}