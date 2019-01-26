using System.Threading.Tasks;
using MrCMS.Entities.People;
using MrCMS.Models.Auth;

namespace MrCMS.Services.Auth
{
    public interface IRegistrationService
    {
        Task<User> RegisterUser(RegisterModel model);
        bool CheckEmailIsNotRegistered(string email);
    }
}