using System;
using System.Web.Mvc;
using MrCMS.Logging;
using MrCMS.Paging;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class LogController : MrCMSAdminController
    {
        private readonly ILogService _service;

        public LogController(ILogService service)
        {
            _service = service;
        }

        [HttpGet]
        public ViewResult Index(LogSearchQuery searchQuery)
        {
            ViewData["query"] = searchQuery;
            var model = _service.GetEntriesPaged(searchQuery);
            return View(model);
        }

        [HttpPost]
        [ActionName("Index")]
        public RedirectToRouteResult Index_Post(LogEntryType? type)
        {
            return RedirectToAction("Index", new {type});
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
            _service.DeleteAllLogs();

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
            _service.DeleteLog(log);

            return RedirectToAction("Index");
        }
    }
}