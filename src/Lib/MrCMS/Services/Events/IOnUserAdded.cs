using MrCMS.Events;
using MrCMS.Services.Events.Args;

namespace MrCMS.Services.Events
{
    public interface IOnUserAdded : IEvent<OnUserAddedEventArgs>
    {
    }
}