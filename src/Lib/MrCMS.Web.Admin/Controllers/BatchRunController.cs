using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Admin.Infrastructure.BaseControllers;
using MrCMS.Web.Admin.Services.Batching;

namespace MrCMS.Web.Admin.Controllers
{
    public class BatchRunController : MrCMSAdminController
    {
        private readonly IBatchRunUIService _batchRunUIService;

        public BatchRunController(IBatchRunUIService batchRunUIService)
        {
            _batchRunUIService = batchRunUIService;
        }

        public async Task<ViewResult> Show(int id)
        {
            var batchRun = await _batchRunUIService.Get(id);
            return View(batchRun);
        }

        public async Task<PartialViewResult> ShowPartial(int id)
        {
            var batchRun = await _batchRunUIService.Get(id);
            return PartialView("Show", batchRun);
        }

        [HttpPost]
        public async Task<JsonResult> Start(int id)
        {
            return Json(await _batchRunUIService.Start(id));
        }

        [HttpPost]
        public async Task<JsonResult> Pause(int id)
        {
            return Json(await _batchRunUIService.Pause(id));
        }

        public async Task<ActionResult> Status(int id)
        {
            var batchRun = await _batchRunUIService.Get(id);
            ViewData["completion-status"] = await _batchRunUIService.GetCompletionStatus(batchRun);
            return PartialView(batchRun);
        }

        public async Task<ActionResult> Row(int id)
        {
            var batchRun = await _batchRunUIService.Get(id);
            ViewData["completion-status"] = await _batchRunUIService.GetCompletionStatus(batchRun);
            return PartialView(batchRun);
        }
    }
}