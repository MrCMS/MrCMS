using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MrCMS.Entities.People;

namespace MrCMS.Models.Auth
{
    public class ResetPasswordViewModel
    {
        public ResetPasswordViewModel()
        {
        }

        public ResetPasswordViewModel(Guid id, User user)
        {
            Id = id;
            Email = user.Email;
        }

        public Guid Id { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Minimum length for password is {2} characters.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Password does not match.")]
        [DisplayName("Confirm Password")]
        public string ConfirmPassword { get; set; }
    }
}