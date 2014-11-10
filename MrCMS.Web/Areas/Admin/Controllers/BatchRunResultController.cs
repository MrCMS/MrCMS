using System.Web.Mvc;
using MrCMS.Batching.Entities;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class BatchRunResultController : MrCMSAdminController
    {
        public PartialViewResult Show(BatchRunResult result)
        {
            return PartialView(result);
        }
    }
}