using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using MrCMS.Entities.People;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.MessageTemplates;
using MrCMS.Website.Filters;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;

namespace MrCMS.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ResendEmailConfirmationModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly IMessageParser<ConfirmEmailMessageTemplate, ConfirmEmailEmailModel> _confirmEmailMessageParser;
        private readonly ISiteUrlResolver _siteUrlResolver;

        public ResendEmailConfirmationModel(UserManager<User> userManager, ISiteUrlResolver siteUrlResolver, IMessageParser<ConfirmEmailMessageTemplate, ConfirmEmailEmailModel> confirmEmailMessageParser)
        {
            _userManager = userManager;
            _confirmEmailMessageParser = confirmEmailMessageParser;
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

        public void OnGet()
        {
        }

        [GoogleRecaptcha]
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Verification email sent. Please check your email.");
                return Page();
            }

            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            var queuedMessage = await _confirmEmailMessageParser.GetMessage(new ConfirmEmailEmailModel
            {
                UserId = userId,
                Code = code,
                Name = user.Name,
                Email = user.Email,
                SiteUrl = _siteUrlResolver.GetCurrentSiteUrl()
            });
            await _confirmEmailMessageParser.QueueMessage(queuedMessage, trySendImmediately: true);

            ModelState.AddModelError(string.Empty, "Verification email sent. Please check your email.");
            return Page();
        }
    }
}
