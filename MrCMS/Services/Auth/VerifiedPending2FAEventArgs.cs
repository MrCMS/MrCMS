using MrCMS.Entities.People;

namespace MrCMS.Services.Auth
{
    public class VerifiedPending2FAEventArgs
    {
        public VerifiedPending2FAEventArgs(User user)
        {
            User = user;
        }

        public User User { get; }
    }
}