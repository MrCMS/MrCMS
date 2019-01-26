using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Apps.Admin.Helpers;
using MrCMS.Web.Apps.Admin.Models;
using MrCMS.Web.Apps.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Admin.Controllers
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
        public RedirectToActionResult Import()
        {
            var summary = _stringResourceUpdateService.Import(Request.Form.Files[0]);
            TempData.SuccessMessages()
                .Add(string.Format("{0} resourced procesed - {1} added, {2} updated",
                    summary.Processed, summary.Added, summary.Updated));
            return RedirectToAction("Index", "Resource");
        }
    }
}