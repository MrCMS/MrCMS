using MrCMS.Entities.People;
using MrCMS.Web.Apps.Core.Models;
using MrCMS.Web.Apps.Core.Models.RegisterAndLogin;

namespace MrCMS.Web.Apps.Core.Services
{
    public interface IRegistrationService
    {
        User RegisterUser(RegisterModel model);
        bool CheckEmailIsNotRegistered(string email);
    }
}