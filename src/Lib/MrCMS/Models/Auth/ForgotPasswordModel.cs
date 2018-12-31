using System.ComponentModel.DataAnnotations;
using MrCMS.Helpers.Validation;

namespace MrCMS.Models.Auth
{
    public class ForgotPasswordModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}