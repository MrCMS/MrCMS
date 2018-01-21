using System;
using MrCMS.Entities.People;

namespace MrCMS.Services.Auth
{
    public class UserLoggedInEventArgs : EventArgs
    {
        public UserLoggedInEventArgs(User user, Guid previousSession)
        {
            User = user;
            PreviousSession = previousSession;
        }

        public User User { get; set; }
        public Guid PreviousSession { get; set; }
    }
}