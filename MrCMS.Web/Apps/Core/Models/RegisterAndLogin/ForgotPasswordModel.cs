using System.ComponentModel.DataAnnotations;
using MrCMS.Helpers.Validation;

namespace MrCMS.Web.Apps.Core.Models.RegisterAndLogin
{
    public class ForgotPasswordModel
    {
        [Required]
        [EmailValidator]
        public string Email { get; set; }
    }
}