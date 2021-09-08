using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Admin.Infrastructure.BaseControllers;
using MrCMS.Web.Admin.Models;
using MrCMS.Web.Admin.Services;

namespace MrCMS.Web.Admin.Controllers
{
    public class PushNotificationController : MrCMSAdminController
    {
        private readonly IPushNotificationAdminService _service;

        public PushNotificationController(IPushNotificationAdminService service)
        {
            _service = service;
        }

        public async Task<ViewResult> Index(PushNotificationSearchModel searchModel)
        {
            ViewData["results"] = await _service.Search(searchModel);
            return View(searchModel);
        }

        public ViewResult SendToAll(PushNotificationModel model)
        {
            ModelState.Clear();
            return View(model);
        }

        public ViewResult SendToRole(PushToRoleNotificationModel model)
        {
            ModelState.Clear();
            return View(model);
        }

        [ActionName(nameof(SendToAll)), HttpPost]
        public async Task<JsonResult> PushToAll(PushNotificationModel model)
        {
            return Json(await _service.PushToAll(model));
        }

        [ActionName(nameof(SendToRole)), HttpPost]
        public async Task<JsonResult> PushToRole(PushToRoleNotificationModel model)
        {
            return Json(await _service.PushToRole(model));
        }
    }
}