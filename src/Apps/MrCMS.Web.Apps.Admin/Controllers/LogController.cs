using Microsoft.AspNetCore.Mvc;
using MrCMS.Logging;
using MrCMS.Web.Apps.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Admin.Controllers
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
        public ActionResult Delete()
        {
            _adminService.DeleteAllLogs();

            return RedirectToAction("Index");
        }

        [HttpGet]
        [ActionName("DeleteLog")]
        public ActionResult DeleteLog_Get(Log log)
        {
            return PartialView(log);
        }

        [HttpPost]
        public ActionResult DeleteLog(int id)
        {
            _adminService.DeleteLog(id);

            return RedirectToAction("Index");
        }
    }
}