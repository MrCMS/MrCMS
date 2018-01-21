using MrCMS.Events;

namespace MrCMS.Services.Auth
{
    public interface IOnUserRegistered : IEvent<OnUserRegisteredEventArgs>
    {
    }
}