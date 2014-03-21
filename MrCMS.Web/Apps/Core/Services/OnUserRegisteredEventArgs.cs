using MrCMS.Entities.People;

namespace MrCMS.Web.Apps.Core.Services
{
    public class OnUserRegisteredEventArgs
    {
        public OnUserRegisteredEventArgs(User user)
        {
            User = user;
        }
        public User User { get; set; }
    }
}