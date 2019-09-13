using Microsoft.AspNetCore.Mvc;
using MrCMS.Services;

namespace MrCMS.Website.Controllers
{
    public class LogoutController : MrCMSUIController
    {
        public const string RouteUrl = "logout";
        private readonly IAuthorisationService _authorisationService;

        public LogoutController(IAuthorisationService authorisationService)
        {
            _authorisationService = authorisationService;
        }


        [HttpGet]
        [Route(RouteUrl)]
        public RedirectResult Logout()
        {
            _authorisationService.Logout();
            return Redirect("~/");
        }
    }
}