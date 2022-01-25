using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MrCMS.Entities.People;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.MessageTemplates;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace MrCMS.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly IMessageParser<ResetPasswordMessageTemplate, ResetPasswordEmailModel> _messageParser;
        private readonly ISiteUrlResolver _siteUrlResolver;

        public ForgotPasswordModel(UserManager<User> userManager,
            IMessageParser<ResetPasswordMessageTemplate, ResetPasswordEmailModel> messageParser,
            ISiteUrlResolver siteUrlResolver)
        {
            _userManager = userManager;
            _messageParser = messageParser;
            _siteUrlResolver = siteUrlResolver;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please 
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);

                var message = await _messageParser.GetMessage(new ResetPasswordEmailModel
                {
                    Code = code,
                    Email = user.Email,
                    Name = user.Name,
                    SiteUrl = _siteUrlResolver.GetCurrentSiteUrl()
                });
                await _messageParser.QueueMessage(message, trySendImmediately: true);

                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }
    }
}
