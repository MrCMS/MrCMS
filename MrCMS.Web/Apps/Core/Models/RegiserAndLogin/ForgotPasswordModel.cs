using System.ComponentModel.DataAnnotations;
using MrCMS.Helpers.Validation;

namespace MrCMS.Web.Apps.Core.Models.RegiserAndLogin
{
    public class ForgotPasswordModel
    {
        [Required]
        [EmailValidator]
        public string Email { get; set; }
    }
}