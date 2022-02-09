using MrCMS.Entities.People;

namespace MrCMS.Services.Events.Args
{
    public class OnUserAddedEventArgs
    {
        public OnUserAddedEventArgs(User user)
        {
            User = user;
        }
        public User User { get; set; }
    }
}