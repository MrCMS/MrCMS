using MrCMS.Entities.People;

namespace MrCMS.Services
{
    public interface IUserManagementService
    {
        void AddUser(User user);
        void SaveUser(User user);
        User GetUser(int id);
        void DeleteUser(int id);
        bool IsUniqueEmail(string email, int? id = null);
    }
}