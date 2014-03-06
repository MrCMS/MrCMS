using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MrCMS.Web.Apps.Core.Models.RegisterAndLogin
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
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid")]
        public string Email { get; set; }
    }
}