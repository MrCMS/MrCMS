using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace MrCMS.Models.Auth
{
    public class UserAccountModel
    {
        [Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [StringLength(128, MinimumLength = 5)]
        [Remote("IsUniqueEmail", "UserAccount", ErrorMessage = "This email is already registered.")]
        [EmailAddress]
        public string Email { get; set; }
    }
}