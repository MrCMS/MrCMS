using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Batching.Entities;
using MrCMS.Data;
using MrCMS.Web.Admin.Infrastructure.BaseControllers;

namespace MrCMS.Web.Admin.Controllers
{
    public class BatchJobController : MrCMSAdminController
    {
        private readonly IRepository<BatchJob> _batchJonRepo;

        public BatchJobController(IRepository<BatchJob> batchJonRepo)
        {
            _batchJonRepo = batchJonRepo;
        }
        public async Task<ActionResult> Row(int id)
        {
            var batchJob = await _batchJonRepo.Get(id);
            return PartialView(batchJob);
        } 
    }
}