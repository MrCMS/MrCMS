using System.Threading.Tasks;
using MrCMS.Entities.People;

namespace MrCMS.Services
{
    public interface IAuthorisationService
    {
        Task SetAuthCookie(User user, bool rememberMe);
        void Logout();
    }
}