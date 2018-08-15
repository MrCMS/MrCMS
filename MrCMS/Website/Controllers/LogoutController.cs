using Microsoft.AspNetCore.Mvc;
using MrCMS.Services;

namespace MrCMS.Website.Controllers
{
    public class LogoutController : MrCMSUIController
    {
        private readonly IAuthorisationService _authorisationService;

        public LogoutController(IAuthorisationService authorisationService)
        {
            _authorisationService = authorisationService;
        }


        [Route("logout")]
        public RedirectResult Logout()
        {
            _authorisationService.Logout();
            return Redirect("~/");
        }
    }
}