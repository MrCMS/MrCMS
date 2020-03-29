using Microsoft.AspNetCore.Mvc;
using MrCMS.Batching.Entities;
using MrCMS.Web.Areas.Admin.Services.Batching;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
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
        public JsonResult Start(int id)
        {
            return Json(_batchRunUIService.Start(id));
        }

        [HttpPost]
        public JsonResult Pause(int id)
        {
            return Json(_batchRunUIService.Pause(id));
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