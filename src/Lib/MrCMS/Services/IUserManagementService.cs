using System.Threading.Tasks;
using MrCMS.Entities.People;

namespace MrCMS.Services
{
    public interface IUserManagementService
    {
        Task AddUser(User user);
        Task SaveUser(User user);
        Task<User> GetUser(int id);
        Task DeleteUser(int id);
        Task<bool> IsUniqueEmail(string email, int? id = null);
    }
}