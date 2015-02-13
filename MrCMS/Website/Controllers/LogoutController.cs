using System.Web.Mvc;
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

        public RedirectResult Logout()
        {
            _authorisationService.Logout();
            return Redirect("~");
        }
    }
}