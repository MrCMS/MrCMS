using System.Web.Mvc;
using MrCMS.Entities.Documents;
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

        public PartialViewResult Show(Document document, int page = 1)
        {
            return PartialView(_service.GetVersions(document, page));
        }
    }
}