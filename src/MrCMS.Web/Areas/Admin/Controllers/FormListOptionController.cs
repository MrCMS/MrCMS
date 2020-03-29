using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
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
        public async Task<ActionResult> Add(AddFormListOptionModel model)
        {
            await _adminService.Add(model);
            return Json(new FormActionResult { success = true });
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {
            return View(await _adminService.GetUpdateModel(id));
        }

        [HttpPost]
        [ActionName(nameof(Edit))]
        public async Task<ActionResult> Edit_POST(UpdateFormListOptionModel model)
        {
            await _adminService.Update(model);
            return Json(new FormActionResult { success = true });
        }

        [HttpGet]
        public async Task<ActionResult> Delete(int id)
        {
            return View(await _adminService.GetUpdateModel(id));
        }

        [HttpPost]
        [ActionName(nameof(Delete))]
        public async Task<ActionResult> Delete_POST(int id)
        {
            await _adminService.Delete(id);
            return Json(new FormActionResult { success = true });
        }

    }
}