using System.Web.Mvc;
using System.Web.UI;
using MrCMS.Batching.Entities;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class BatchController : MrCMSAdminController
    {
        private readonly IBatchAdminService _batchAdminService;

        public BatchController(IBatchAdminService batchAdminService)
        {
            _batchAdminService = batchAdminService;
        }

        public ViewResult Index(BatchSearchModel searchModel)
        {
            ViewData["results"] = _batchAdminService.Search(searchModel);
            return View(searchModel);
        }

        public ViewResult Show(Batch batch)
        {
            return View(batch);
        }

        public ActionResult ShowPartial(Batch batch)
        {
            return PartialView("Show", batch);
        }
    }
}