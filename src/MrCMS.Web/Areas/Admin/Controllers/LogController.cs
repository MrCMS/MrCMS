using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Logging;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class LogController : MrCMSAdminController
    {
        private readonly ILogAdminService _adminService;

        public LogController(ILogAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet]
        public ViewResult Index(LogSearchQuery searchQuery)
        {
            ViewData["site-options"] = _adminService.GetSiteOptions();
            ViewData["logs"] = _adminService.GetEntriesPaged(searchQuery);
            return View(searchQuery);
        }

        public ViewResult Show(Log log)
        {
            return View(log);
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
        public ActionResult DeleteLog_Get(Log log)
        {
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