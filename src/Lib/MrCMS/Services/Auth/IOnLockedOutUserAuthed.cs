using MrCMS.Events;

namespace MrCMS.Services.Auth
{
    public interface IOnLockedOutUserAuthed : IEvent<UserLockedOutEventArgs>
    {
    }
    public interface IOnFailedLogin : IEvent<UserFailedLoginEventArgs>
    {
    }
}