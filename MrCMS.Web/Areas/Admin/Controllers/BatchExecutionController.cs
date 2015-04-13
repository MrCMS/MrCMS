using System.Threading.Tasks;
using System.Web.Mvc;
using MrCMS.Batching.Entities;
using MrCMS.Web.Areas.Admin.ModelBinders;
using MrCMS.Web.Areas.Admin.Services.Batching;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class BatchExecutionController : MrCMSAdminController
    {
        private readonly IBatchRunUIService _batchRunUIService;

        public BatchExecutionController(IBatchRunUIService batchRunUIService)
        {
            _batchRunUIService = batchRunUIService;
        }

        public async Task<JsonResult> ExecuteNext([IoCModelBinder(typeof(BatchRunGuidModelBinder))]BatchRun run)
        {
            var result = run == null ? null : await _batchRunUIService.ExecuteNextTask(run);
            if (result != null)
            {
                _batchRunUIService.ExecuteRequestForNextTask(run);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}