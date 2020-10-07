using MrCMS.Events;

namespace MrCMS.Services.Auth
{
    public interface IOnLoggedOut : IEvent<LoggedOutEventArgs>
    {
    }
}