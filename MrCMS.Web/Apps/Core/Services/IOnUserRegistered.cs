using MrCMS.Entities.People;
using MrCMS.Events;

namespace MrCMS.Web.Apps.Core.Services
{
    public interface IOnUserRegistered : IEvent<OnUserRegisteredEventArgs>
    {
    }

    public class OnUserRegisteredEventArgs
    {
        public OnUserRegisteredEventArgs(User user)
        {
            User = user;
        }
        public User User { get; set; }
    }
}