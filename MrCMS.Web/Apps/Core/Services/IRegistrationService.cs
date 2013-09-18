using MrCMS.Web.Apps.Core.Models;

namespace MrCMS.Web.Apps.Core.Services
{
    public interface IRegistrationService
    {
        void RegisterUser(RegisterModel model);
        bool CheckEmailIsNotRegistered(string email);
    }
}