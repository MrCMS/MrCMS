using System;
using System.Threading.Tasks;
using MrCMS.Entities.People;
using MrCMS.Web.Apps.Core.Models;
using MrCMS.Web.Apps.Core.Models.RegisterAndLogin;

namespace MrCMS.Web.Apps.Core.Services
{
    public interface ILoginService
    {
        Task<LoginResult> AuthenticateUser(LoginModel loginModel);
    }
}