using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Admin.Infrastructure.BaseControllers;
using MrCMS.Web.Admin.Models;
using MrCMS.Web.Admin.Services;

namespace MrCMS.Web.Admin.Controllers
{
    public class BatchController : MrCMSAdminController
    {
        private readonly IBatchAdminService _batchAdminService;

        public BatchController(IBatchAdminService batchAdminService)
        {
            _batchAdminService = batchAdminService;
        }

        public async Task<ViewResult> Index(BatchSearchModel searchModel)
        {
            ViewData["results"] = await _batchAdminService.Search(searchModel);
            return View(searchModel);
        }

        public async Task<ViewResult> Show(int id)
        {
            var batch = await _batchAdminService.Get(id);
            return View(batch);
        }

        public async Task<ActionResult> ShowPartial(int id)
        {
            var batch = await _batchAdminService.Get(id);
            return PartialView("Show", batch);
        }
    }
}