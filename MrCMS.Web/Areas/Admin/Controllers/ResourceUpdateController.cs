using System.Web.Mvc;
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
            _stringResourceUpdateService.Import(Request.Files[0]);
            return RedirectToAction("Index", "Resource");
        }
    }
}