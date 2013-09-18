using MrCMS.Entities.People;

namespace MrCMS.Web.Apps.Core.Services
{
    public interface IOnUserRegistered
    {
        void UserRegistered(User user);
    }
}