using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Admin.Models;
using MrCMS.Web.Admin.Services;
using MrCMS.Web.Admin.Infrastructure.BaseControllers;
using MrCMS.Web.Admin.Infrastructure.Helpers;

namespace MrCMS.Web.Admin.Controllers
{
    public class ResourceUpdateController : MrCMSAdminController
    {
        private readonly IStringResourceUpdateService _stringResourceUpdateService;

        public ResourceUpdateController(IStringResourceUpdateService stringResourceUpdateService)
        {
            _stringResourceUpdateService = stringResourceUpdateService;
        }

        public async Task<FileResult> Export(StringResourceSearchQuery searchQuery)
        {
            return await _stringResourceUpdateService.Export(searchQuery);
        }

        [HttpPost]
        public async Task<RedirectToActionResult> Import()
        {
            var summary = await _stringResourceUpdateService.Import(Request.Form.Files[0]);
            TempData.AddSuccessMessage(
                $"{summary.Processed} resourced procesed - {summary.Added} added, {summary.Updated} updated");
            return RedirectToAction("Index", "Resource");
        }
    }
}