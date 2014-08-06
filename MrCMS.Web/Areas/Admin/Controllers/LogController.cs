using System.Web.Mvc;
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

        public ViewResult Show(Log entry)
        {
            return View(entry);
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
        public ActionResult DeleteLog(Log log)
        {
            _adminService.DeleteLog(log);

            return RedirectToAction("Index");
        }
    }
}