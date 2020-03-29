using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class VersionsController : MrCMSAdminController
    {
        private readonly IDocumentVersionsAdminService _service;

        public VersionsController(IDocumentVersionsAdminService service)
        {
            _service = service;
        }

        [HttpGet]
        public PartialViewResult Revert(int id)
        {
            return PartialView(_service.GetDocumentVersion(id));
        }

        [HttpPost]
        [ActionName("Revert")]
        public async Task<RedirectToActionResult> Revert_POST(int id)
        {
            var version = await _service.RevertToVersion(id);
            return RedirectToAction("Edit", "Webpage", new { id = version.Document.Id });
        }
    }
}