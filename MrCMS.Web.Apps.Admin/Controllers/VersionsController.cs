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
        public PartialViewResult Revert(DocumentVersion documentVersion)
        {
            return PartialView(documentVersion);
        }

        [HttpPost]
        [ActionName("Revert")]
        public RedirectToActionResult Revert_POST(DocumentVersion documentVersion)
        {
            _service.RevertToVersion(documentVersion);
            return RedirectToAction("Edit", "Webpage", new { id = documentVersion.Document.Id });
        }
    }
}