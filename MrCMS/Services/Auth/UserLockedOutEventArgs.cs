using MrCMS.Entities.People;

namespace MrCMS.Services.Auth
{
    public class UserLockedOutEventArgs
    {
        public UserLockedOutEventArgs(User user)
        {
            User = user;
        }

        public User User { get; set; }
    }
}