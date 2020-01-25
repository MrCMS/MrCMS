using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Apps.Admin.Models;
using MrCMS.Web.Apps.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Admin.Controllers
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
        public async Task<JsonResult> Add(AddFormPropertyModel model)
        {
            await _adminService.Add(model);
            return Json(new FormActionResult { success = true });
        }

        [HttpGet]
        public async Task<ViewResult> Edit(int id)
        {
            return View(await _adminService.GetUpdateModel(id));
        }

        [HttpPost]
        [ActionName(nameof(Edit))]
        public async Task<JsonResult> Edit_POST(UpdateFormPropertyModel model)
        {
            await _adminService.Update(model);
            return Json(new FormActionResult { success = true });
        }

        [HttpGet]
        public async Task<ViewResult> Delete(int id)
        {
            return View(await _adminService.GetUpdateModel(id));
        }
        [HttpPost]
        [ActionName(nameof(Delete))]
        public async Task<JsonResult> Delete_POST(int id)
        {
            await _adminService.Delete(id);
            return Json(new FormActionResult { success = true });
        }

    }
}