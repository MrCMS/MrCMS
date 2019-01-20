using System.Threading.Tasks;
using MrCMS.Entities.People;

namespace MrCMS.Services.Auth
{
    public interface ILogUserIn
    {
        Task Login(User user, bool rememberMe);
    }
}