using Microsoft.AspNetCore.Mvc;

namespace MrCMS.Website.Controllers
{
    public class LogoutController : MrCMSUIController
    {
        //private readonly IAuthorisationService _authorisationService;

        //public LogoutController(IAuthorisationService authorisationService)
        //{
        //    _authorisationService = authorisationService;
        //}

        public RedirectResult Logout()
        {
            // TODO: logout
            //_authorisationService.Logout();
            return Redirect("~");
        }
    }
}