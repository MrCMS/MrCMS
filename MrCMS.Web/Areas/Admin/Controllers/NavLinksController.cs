using System.Web.Mvc;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class NavLinksController : MrCMSAdminController
    {
        private readonly IAdminNavLinksService _navLinksService;

        public NavLinksController(IAdminNavLinksService navLinksService)
        {
            _navLinksService = navLinksService;
        }

        public PartialViewResult Get()
        {
            return PartialView(_navLinksService.GetNavLinks());
        }
    }
}