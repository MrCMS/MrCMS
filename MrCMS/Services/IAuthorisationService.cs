using MrCMS.Entities.People;

namespace MrCMS.Services
{
    public interface IAuthorisationService
    {
        void ValidatePassword(string password, string confirmation);
        void SetPassword(User user, string password, string confirmation);
        bool ValidateUser(User user, string password);
        void SetAuthCookie(string email, bool rememberMe);
        void Logout();
    }
}