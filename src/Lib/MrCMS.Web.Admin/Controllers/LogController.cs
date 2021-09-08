using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Logging;
using MrCMS.Web.Admin.Infrastructure.BaseControllers;
using MrCMS.Web.Admin.Services;

namespace MrCMS.Web.Admin.Controllers
{
    public class LogController : MrCMSAdminController
    {
        private readonly ILogAdminService _adminService;

        public LogController(ILogAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet]
        public async Task<ViewResult> Index(LogSearchQuery searchQuery)
        {
            ViewData["site-options"] = await _adminService.GetSiteOptions();
            ViewData["logs"] = await _adminService.GetEntriesPaged(searchQuery);
            return View(searchQuery);
        }

        public async Task<ViewResult> Show(int id)
        {
            return View(await _adminService.Get(id));
        }

        [HttpGet]
        [ActionName("Delete")]
        public ActionResult Delete_Get()
        {
            return PartialView();
        }

        [HttpPost]
        public async Task<ActionResult> Delete()
        {
            await _adminService.DeleteAllLogs();

            return RedirectToAction("Index");
        }

        [HttpGet]
        [ActionName("DeleteLog")]
        public async Task<ActionResult> DeleteLog_Get(int id)
        {
            var log = await _adminService.Get(id);
            return PartialView(log);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteLog(int id)
        {
            await _adminService.DeleteLog(id);

            return RedirectToAction("Index");
        }
    }
}