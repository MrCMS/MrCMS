using Microsoft.AspNetCore.Mvc;
using MrCMS.Batching.Entities;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Admin.Controllers
{
    public class BatchJobController : MrCMSAdminController
    {
        public ActionResult Row(BatchJob batchJob)
        {
            return PartialView(batchJob);
        } 
    }
}