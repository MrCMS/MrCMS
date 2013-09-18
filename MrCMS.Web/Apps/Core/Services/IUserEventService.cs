using MrCMS.Entities.People;

namespace MrCMS.Web.Apps.Core.Services
{
    public interface IUserEventService
    {
        void OnUserRegistered(User user);
    }
}