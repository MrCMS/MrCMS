using Microsoft.AspNetCore.Mvc;
using MrCMS.Batching.Entities;
using MrCMS.Web.Apps.Admin.Services.Batching;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Admin.Controllers
{
    public class BatchRunController : MrCMSAdminController
    {
        private readonly IBatchRunUIService _batchRunUIService;

        public BatchRunController(IBatchRunUIService batchRunUIService)
        {
            _batchRunUIService = batchRunUIService;
        }

        public ViewResult Show(BatchRun batchRun)
        {
            return View(batchRun);
        }

        public PartialViewResult ShowPartial(BatchRun batchRun)
        {
            return PartialView("Show", batchRun);
        }

        [HttpPost]
        public JsonResult Start(BatchRun run)
        {
            return Json(_batchRunUIService.Start(run));
        }

        [HttpPost]
        public JsonResult Pause(BatchRun run)
        {
            return Json(_batchRunUIService.Pause(run));
        }

        public ActionResult Status(BatchRun batchRun)
        {
            ViewData["completion-status"] = _batchRunUIService.GetCompletionStatus(batchRun);
            return PartialView(batchRun);
        }

        public ActionResult Row(BatchRun batchRun)
        {
            ViewData["completion-status"] = _batchRunUIService.GetCompletionStatus(batchRun);
            return PartialView(batchRun);
        }
    }
}