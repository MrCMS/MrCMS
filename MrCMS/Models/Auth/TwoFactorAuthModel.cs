using System.ComponentModel.DataAnnotations;

namespace MrCMS.Models.Auth
{
    public class TwoFactorAuthModel
    {
        [Required]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }
    }
}