using System.Threading.Tasks;
using MrCMS.Entities.People;

namespace MrCMS.Services
{
    public interface IExternalUserSource
    {
        string Name { get; }
        Task<User> SynchroniseUser(string email);
        Task<bool> ValidateUser(User user, string password);
        Task UpdateFromSource(User user);
    }
}