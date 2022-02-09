using System;
using MrCMS.Entities.People;

namespace MrCMS.Services.Auth
{
    public class OnUserRegisteredEventArgs
    {
       public OnUserRegisteredEventArgs(User user, Guid previousSession)
        {
            User = user;
            PreviousSession = previousSession;
        }

        public User User { get; set; }
        public Guid PreviousSession { get; set; }
    }
}