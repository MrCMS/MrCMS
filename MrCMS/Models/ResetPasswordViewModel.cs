using System;
using MrCMS.Entities.People;

namespace MrCMS.Models
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
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}