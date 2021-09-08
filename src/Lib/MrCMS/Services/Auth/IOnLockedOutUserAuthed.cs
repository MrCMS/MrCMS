using MrCMS.Events;

namespace MrCMS.Services.Auth
{
    public interface IOnLockedOutUserAuthed : IEvent<UserLockedOutEventArgs>
    {
    }
}