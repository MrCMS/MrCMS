using System.Threading.Tasks;
using MrCMS.Models.Auth;

namespace MrCMS.Services.Auth
{
    public interface ILoginService
    {
        LoginResult AuthenticateUser(LoginModel loginModel);
    }
}