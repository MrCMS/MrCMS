using MrCMS.Entities.People;
using MrCMS.Web.Apps.Core.Models;

namespace MrCMS.Web.Apps.Core.Services
{
    public interface IRegistrationService
    {
        User RegisterUser(RegisterModel model);
        bool CheckEmailIsNotRegistered(string email);
    }
}