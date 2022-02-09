using System.Threading.Tasks;
using MrCMS.Entities.People;

namespace MrCMS.Services
{
    public interface IPasswordEncryptionManager
    {
        Task UpdateEncryption(User user, string password);
        void SetPassword(User user, string password);
        bool ValidateUser(User user, string password);
    }
}