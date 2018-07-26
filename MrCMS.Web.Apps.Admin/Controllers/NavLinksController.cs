using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Apps.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Admin.Controllers
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