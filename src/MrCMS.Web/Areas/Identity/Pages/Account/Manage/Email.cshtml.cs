using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MrCMS.Entities.People;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.MessageTemplates;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace MrCMS.Web.Areas.Identity.Pages.Account.Manage
{
    public partial class EmailModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly
            IMessageParser<ConfirmEmailChangeMessageTemplate, ConfirmEmailChangeEmailModel>
            _confirmEmailChangeMessageParser;

        private readonly IMessageParser<ConfirmEmailMessageTemplate, ConfirmEmailEmailModel> _confirmEmailMessageParser;
        private readonly ISiteUrlResolver _siteUrlResolver;

        public EmailModel(
            UserManager<User> userManager,
            IMessageParser<ConfirmEmailChangeMessageTemplate, ConfirmEmailChangeEmailModel>
            confirmEmailChangeMessageParser,
            IMessageParser<ConfirmEmailMessageTemplate, ConfirmEmailEmailModel> confirmEmailMessageParser,
            ISiteUrlResolver siteUrlResolver
            )
        {
            _userManager = userManager;
            _confirmEmailChangeMessageParser = confirmEmailChangeMessageParser;
            _confirmEmailMessageParser = confirmEmailMessageParser;
            _siteUrlResolver = siteUrlResolver;
        }

        public string Username { get; set; }

        public string Email { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "New email")]
            public string NewEmail { get; set; }
        }

        private async Task LoadAsync(User user)
        {
            var email = await _userManager.GetEmailAsync(user);
            Email = email;

            Input = new InputModel
            {
                NewEmail = email,
            };

            IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostChangeEmailAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var email = await _userManager.GetEmailAsync(user);
            if (Input.NewEmail != email)
            {
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateChangeEmailTokenAsync(user, Input.NewEmail);

                var message = await _confirmEmailChangeMessageParser.GetMessage(new ConfirmEmailChangeEmailModel
                {
                    Email = user.Email,
                    NewEmail = Input.NewEmail,
                    Code = code,
                    Name = user.Name,
                    UserId = userId,
                    SiteUrl = _siteUrlResolver.GetCurrentSiteUrl()
                });
                await _confirmEmailChangeMessageParser.QueueMessage(message, trySendImmediately: true);

                StatusMessage = "Confirmation link to change email sent. Please check your email.";
                return RedirectToPage();
            }

            StatusMessage = "Your email is unchanged.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostSendVerificationEmailAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var userId = await _userManager.GetUserIdAsync(user);
            var email = await _userManager.GetEmailAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var queuedMessage = await _confirmEmailMessageParser.GetMessage(new ConfirmEmailEmailModel
            {
                UserId = userId,
                Code = code,
                Name = user.Name,
                Email = email,
                SiteUrl = _siteUrlResolver.GetCurrentSiteUrl()
            });
            await _confirmEmailMessageParser.QueueMessage(queuedMessage, trySendImmediately: true);

            StatusMessage = "Verification email sent. Please check your email.";
            return RedirectToPage();
        }
    }
}
