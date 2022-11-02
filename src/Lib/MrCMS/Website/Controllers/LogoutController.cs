using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Services;
using MrCMS.Services.Auth;

namespace MrCMS.Website.Controllers
{
    public class LogoutController : MrCMSUIController
    {
        private readonly ISignInManager _signInManager;
        private readonly IEventContext _eventContext;
        public const string RouteUrl = "logout";

        public LogoutController(ISignInManager signInManager, IEventContext eventContext)
        {
            _signInManager = signInManager;
            _eventContext = eventContext;
        }


        [Route(RouteUrl)]
        public async Task<RedirectResult> Logout()
        {
            var user = User;
            await _signInManager.SignOutAsync();
            await _eventContext.Publish<IOnLoggedOut, LoggedOutEventArgs>(new LoggedOutEventArgs(user));
            return Redirect("~/");
        }
    }
}