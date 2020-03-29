using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace MrCMS.Web.Areas.Admin.Models
{
    public class AddUserModel
    {
        [Required]
        [Remote("IsUniqueEmail", "User")]
        [EmailAddress]
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UICulture { get; set; }
        public bool IsActive { get; set; }
        public bool DisableNotifications { get; set; }
        
        public string Name =>
            (string.IsNullOrEmpty(FirstName) || string.IsNullOrWhiteSpace(LastName)
                ? this.Email
                : this.FirstName + " " + this.LastName);
        
        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, ErrorMessage = "Minimum length for password is {2} characters.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DisplayName("Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password does not match.")]
        public string ConfirmPassword { get; set; }
    }
}