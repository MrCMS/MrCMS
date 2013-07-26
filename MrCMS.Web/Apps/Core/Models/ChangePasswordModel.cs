using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using MrCMS.Entities.People;

namespace MrCMS.Web.Apps.Core.Models
{
    public class ChangePasswordModel
    {
        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, ErrorMessage = "Minimum length for password is {2} characters.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DisplayName("Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password does not match.")]
        public string ConfirmPassword { get; set; }

        public string Message { get; set; }
    }
}