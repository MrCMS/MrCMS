using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Admin.Infrastructure.BaseControllers;
using MrCMS.Web.Admin.Services;

namespace MrCMS.Web.Admin.Controllers
{
    public class VersionsController : MrCMSAdminController
    {
        private readonly IWebpageVersionsAdminService _service;

        public VersionsController(IWebpageVersionsAdminService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<PartialViewResult> Revert(int id)
        {
            return PartialView(await _service.GetDocumentVersion(id));
        }

        [HttpPost]
        [ActionName("Revert")]
        public async Task<RedirectToActionResult> Revert_POST(int id)
        {
            var version = await _service.RevertToVersion(id);
            return RedirectToAction("Edit", "Webpage", new { id = version.Webpage.Id });
        }
    }
}