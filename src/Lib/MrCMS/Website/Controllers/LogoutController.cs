using System.Threading.Tasks;
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


        [Route(RouteUrl)]
        public async Task<RedirectResult> Logout()
        {
            await _authorisationService.Logout();
            return Redirect("~/");
        }
    }
}