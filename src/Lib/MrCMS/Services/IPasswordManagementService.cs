using System.Threading.Tasks;
using MrCMS.Entities.People;

namespace MrCMS.Services
{
    public interface IPasswordManagementService
    {
        bool ValidatePassword(string password, string confirmation);
        void SetPassword(User user, string password, string confirmation);
        Task<bool> ValidateUser(User user, string password);
    }
}