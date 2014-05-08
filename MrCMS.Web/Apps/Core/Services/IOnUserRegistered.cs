using MrCMS.Events;

namespace MrCMS.Web.Apps.Core.Services
{
    public interface IOnUserRegistered : IEvent<OnUserRegisteredEventArgs>
    {
    }
}