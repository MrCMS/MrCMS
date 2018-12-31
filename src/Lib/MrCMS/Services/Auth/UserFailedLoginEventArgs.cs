using MrCMS.Entities.People;

namespace MrCMS.Services.Auth
{
    public class UserFailedLoginEventArgs
    {
        public UserFailedLoginEventArgs(User user,string email)
        {
            User = user;
            Email = email;
        }

        public User User { get; }
        public string Email { get; }
    }
}