using System;
using System.ComponentModel.DataAnnotations;
using MrCMS.Entities.People;

namespace MrCMS.Web.Apps.Core.Models
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