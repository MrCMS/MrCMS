using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using MrCMS.Entities.People;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.Models;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Website;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Controllers
{
    public class ExternalLoginController : MrCMSUIController
    {
        private const string XsrfKey = "XsrfId";
        private readonly IAuthenticationManager _authenticationManager;
        private readonly IExternalLoginService _externalLoginService;
        private readonly IUniquePageService _uniquePageService;

        public ExternalLoginController(IAuthenticationManager authenticationManager,
                                       IExternalLoginService externalLoginService, IUniquePageService uniquePageService)
        {
            _authenticationManager = authenticationManager;
            _externalLoginService = externalLoginService;
            _uniquePageService = uniquePageService;
        }


        [HttpPost]
        public ActionResult Login(string provider, string returnUrl)
        {
            Session.RemoveAll();
            // Request a redirect to the external login provider
            return new ChallengeResult(provider,
                                       Url.Action("Callback", "ExternalLogin",
                                                  new { ReturnUrl = returnUrl ?? CurrentRequestData.HomePage.AbsoluteUrl }));
        }

        public async Task<ActionResult> Callback(string returnUrl)
        {
            ExternalLoginInfo externalLoginInfo = await _authenticationManager.GetExternalLoginInfoAsync();
            AuthenticateResult authenticateResult =
                await _authenticationManager.AuthenticateAsync(DefaultAuthenticationTypes.ExternalCookie);
            string email = _externalLoginService.GetEmail(authenticateResult);
            if (string.IsNullOrWhiteSpace(email))
            {
                TempData["login-model"] = new LoginModel
                                              {
                                                  Message =
                                                      "There was an error retrieving your email from the 3rd party provider"
                                              };
                return _uniquePageService.RedirectTo<LoginPage>();
            }
            if (_externalLoginService.IsLogin(externalLoginInfo))
            {
                _externalLoginService.Login(externalLoginInfo, authenticateResult);
                return _externalLoginService.RedirectAfterLogin(email, returnUrl);
            }
            if (await _externalLoginService.UserExistsAsync(email))
            {
                _externalLoginService.AssociateLoginToUser(email, externalLoginInfo);
                _externalLoginService.Login(externalLoginInfo, authenticateResult);
                return _externalLoginService.RedirectAfterLogin(email, returnUrl);
            }
            if (!_externalLoginService.RequiresAdditionalFieldsForRegistration())
            {
                _externalLoginService.CreateUser(email, externalLoginInfo);
                _externalLoginService.Login(externalLoginInfo, authenticateResult);
                return _externalLoginService.RedirectAfterLogin(email, returnUrl);
            }

            return _uniquePageService.RedirectTo<CompleteExternalRegistrationPage>();

        }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri, string userId = null)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
    }

    public interface IExternalLoginService
    {
        bool IsLogin(ExternalLoginInfo externalLoginInfo);
        void Login(ExternalLoginInfo externalLoginInfo, AuthenticateResult authenticateResult);
        bool UserExists(string authenticateResult);
        void AssociateLoginToUser(string email, ExternalLoginInfo externalLoginInfo);
        bool RequiresAdditionalFieldsForRegistration();
        void CreateUser(string email, ExternalLoginInfo externalLoginInfo);
        ActionResult RedirectAfterLogin(string email, string returnUrl);
        string GetEmail(AuthenticateResult authenticateResult);
    }

    public class ExternalLoginService : IExternalLoginService
    {
        private readonly IAuthorisationService _authorisationService;
        private readonly UserManager<User> _userManager;

        public ExternalLoginService(UserManager<User> userManager, IAuthorisationService authorisationService)
        {
            _userManager = userManager;
            _authorisationService = authorisationService;
        }

        public bool IsLogin(ExternalLoginInfo externalLoginInfo)
        {
            return _userManager.Find(externalLoginInfo.Login) != null;
        }

        public void Login(ExternalLoginInfo externalLoginInfo, AuthenticateResult authenticateResult)
        {
            User user = _userManager.Find(externalLoginInfo.Login);
            _authorisationService.SetAuthCookie(user, false);
            _authorisationService.UpdateClaims(user, authenticateResult.Identity.Claims);
        }

        public bool UserExists(string email)
        {
            return _userManager.FindByName(email) != null;
        }

        public string GetEmail(AuthenticateResult authenticateResult)
        {
            if (authenticateResult != null && authenticateResult.Identity != null)
            {
                IEnumerable<Claim> claims = authenticateResult.Identity.Claims;
                if (claims != null)
                {
                    Claim emailClaim = claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email);
                    if (emailClaim != null)
                        return emailClaim.Value;
                }
            }
            return null;
        }

        public void AssociateLoginToUser(string email, ExternalLoginInfo externalLoginInfo)
        {
            if (string.IsNullOrWhiteSpace(email))
                return;
            var user = _userManager.FindByName(email);
            if (user == null)
                return;
            _userManager.AddLogin(user.OwinId, externalLoginInfo.Login);
        }

        public bool RequiresAdditionalFieldsForRegistration()
        {
            //TODO: wire in framework to allow extra fields to be added

            return false;
        }

        public void CreateUser(string email, ExternalLoginInfo externalLoginInfo)
        {
            var user = new User { Email = email, IsActive = true };
            _userManager.Create(user);
            _userManager.AddLogin(user.OwinId, externalLoginInfo.Login);
        }

        public ActionResult RedirectAfterLogin(string email, string returnUrl)
        {
            var user = _userManager.FindByName(email);
            if (!string.IsNullOrWhiteSpace(returnUrl))
                return new RedirectResult(returnUrl);
            return user.IsAdmin ? new RedirectResult("~/admin") : new RedirectResult("~");
        }
    }

    public static class ExternalLoginServiceExtensions
    {
        public static Task<bool> UserExistsAsync(this IExternalLoginService service, string email)
        {
            return Task.Run(() => service.UserExists(email));
        }
    }
}