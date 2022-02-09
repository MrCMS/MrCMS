using System.ComponentModel.DataAnnotations;

namespace MrCMS.Models.Auth
{
    public class ForgotPasswordModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}