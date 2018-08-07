using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.Documents;
using MrCMS.Web.Apps.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Admin.Controllers
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
        public RedirectToActionResult Revert_POST(int id)
        {
            var version =_service.RevertToVersion(id);
            return RedirectToAction("Edit", "Webpage", new { id = version.Document.Id });
        }
    }
}