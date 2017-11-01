using System;
using MrCMS.Entities.People;

namespace MrCMS.Services.Auth
{
    public class ResetPasswordEventArgs : EventArgs
    {
        public ResetPasswordEventArgs(User user)
        {
            User = user;
        }

        public User User { get; set; }
    }
}