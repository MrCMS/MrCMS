using System.Web.Mvc;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class ResourceUpdateController : MrCMSAdminController
    {
        private readonly IStringResourceUpdateService _stringResourceUpdateService;

        public ResourceUpdateController(IStringResourceUpdateService stringResourceUpdateService)
        {
            _stringResourceUpdateService = stringResourceUpdateService;
        }

        public FileResult Export(StringResourceSearchQuery searchQuery)
        {
            return _stringResourceUpdateService.Export(searchQuery);
        }

        [HttpPost]
        public RedirectToRouteResult Import()
        {
            var summary = _stringResourceUpdateService.Import(Request.Files[0]);
            TempData.SuccessMessages()
                .Add(string.Format("{0} resourced procesed - {1} added, {2} updated",
                    summary.Processed, summary.Added, summary.Updated));
            return RedirectToAction("Index", "Resource");
        }
    }
}