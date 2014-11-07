using System.Web.Mvc;
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
            GetResults(batchRun);
            return View(batchRun);
        }

        public PartialViewResult ShowPartial(BatchRun batchRun)
        {
            GetResults(batchRun);
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

        [HttpPost]
        public JsonResult ExecuteNext(BatchRun run)
        {
            return Json(_batchRunUIService.ExecuteNextTask(run));
        }

        private void GetResults(BatchRun batchRun)
        {
            ViewData["results"] = _batchRunUIService.GetResults(batchRun);
        }

        public ActionResult Status(BatchRun batchRun)
        {
            return PartialView(batchRun);
        }

        public ActionResult Row(BatchRun batchRun)
        {
            return PartialView(batchRun);
        }
    }
}