using System.Collections.Generic;
using System.Security.Claims;
using MrCMS.Entities.People;

namespace MrCMS.Services
{
    public interface IAuthorisationService
    {
        void SetAuthCookie(User user, bool rememberMe);
        void Logout();
        void UpdateClaims(User user, IEnumerable<Claim> claims);
    }

    public interface IPasswordManagementService
    {
        bool ValidatePassword(string password, string confirmation);
        void SetPassword(User user, string password, string confirmation);
        bool ValidateUser(User user, string password);
    }
}