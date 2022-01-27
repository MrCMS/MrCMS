using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using MrCMS.Entities.People;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Web.Apps.Core.MessageTemplates;
using MrCMS.Website.Filters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MrCMS.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IMessageParser<ConfirmEmailMessageTemplate, ConfirmEmailEmailModel> _confirmEmailMessageParser;
        private readonly ISiteUrlResolver _siteUrlResolver;
        private readonly ICheckGoogleRecaptcha _checkGoogleRecaptcha;

        public RegisterModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IMessageParser<ConfirmEmailMessageTemplate, ConfirmEmailEmailModel> confirmEmailMessageParser,
            ISiteUrlResolver siteUrlResolver,
            ICheckGoogleRecaptcha checkGoogleRecaptcha,
            ILogger<RegisterModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _confirmEmailMessageParser = confirmEmailMessageParser;
            _siteUrlResolver = siteUrlResolver;
            _checkGoogleRecaptcha = checkGoogleRecaptcha;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        [GoogleRecaptcha]
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            //Check recaptch
            //todo: IAsyncPageFilter should be implemented for it.
            var recaptchaResult = await _checkGoogleRecaptcha.CheckTokenAsync(this.Request.Form["g-recaptcha-response"]);
            switch (recaptchaResult)
            {
                case GoogleRecaptchaCheckResult.NotEnabled:
                case GoogleRecaptchaCheckResult.Success:
                    // continue
                    break;
                case GoogleRecaptchaCheckResult.Missing:
                    return Content("Please Complete Recaptcha");
                case GoogleRecaptchaCheckResult.Failed:
                    return Content("");
                default:
                    throw new ArgumentOutOfRangeException();
            }

            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new User { Email = Input.Email };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    var queuedMessage = await _confirmEmailMessageParser.GetMessage(new ConfirmEmailEmailModel
                    {
                        UserId = userId,
                        Code = code,
                        Name = user.Name,
                        Email = user.Email,
                        SiteUrl = _siteUrlResolver.GetCurrentSiteUrl()
                    });
                    await _confirmEmailMessageParser.QueueMessage(queuedMessage, trySendImmediately: true);

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
