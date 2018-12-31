using MrCMS.Entities.People;

namespace MrCMS.Services.Auth
{
    public interface ISetVerifiedUserData
    {
        void SetUserData(User user);
    }
}