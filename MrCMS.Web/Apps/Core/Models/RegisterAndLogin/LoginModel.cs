using System.ComponentModel.DataAnnotations;

namespace MrCMS.Web.Apps.Core.Models.RegisterAndLogin
{
    public class LoginModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
        public string ReturnUrl { get; set; }
        public string Message { get; set; }
    }
}