using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Batching.Entities;
using MrCMS.Data;
using MrCMS.Web.Admin.Infrastructure.BaseControllers;

namespace MrCMS.Web.Admin.Controllers
{
    public class BatchRunResultController : MrCMSAdminController
    {
        private readonly IRepository<BatchRunResult> _batchRunResult;

        public BatchRunResultController(IRepository<BatchRunResult> batchRunResult)
        {
            _batchRunResult = batchRunResult;
        }
        public async Task<PartialViewResult> Show(int id)
        {
            return PartialView(await _batchRunResult.Get(id));
        }
    }
}