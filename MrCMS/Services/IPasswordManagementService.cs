using MrCMS.Entities.People;

namespace MrCMS.Services
{
    public interface IPasswordManagementService
    {
        bool ValidatePassword(string password, string confirmation);
        void SetPassword(User user, string password, string confirmation);
        bool ValidateUser(User user, string password);
    }
}