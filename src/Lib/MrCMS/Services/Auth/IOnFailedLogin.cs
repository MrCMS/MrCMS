using MrCMS.Events;

namespace MrCMS.Services.Auth
{
    public interface IOnFailedLogin : IEvent<UserFailedLoginEventArgs>
    {
    }
}