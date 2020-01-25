using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Core.Services
{
    public class PasswordProtectedPageChecker : IPasswordProtectedPageChecker
    {
        private readonly IGetCurrentUser _getCurrentUser;
        private readonly IUserRoleManager _userRoleManager;

        public PasswordProtectedPageChecker(IGetCurrentUser getCurrentUser, IUserRoleManager userRoleManager)
        {
            _getCurrentUser = getCurrentUser;
            _userRoleManager = userRoleManager;
        }

        private string GetCookieKey(Webpage webpage)
        {
            return $"MrCMS.PasswordProtectedPage.{webpage.Id}";
        }
        public async Task<bool> CanAccessPage(Webpage webpage, IRequestCookieCollection cookies)
        {
            var user = _getCurrentUser.Get();
            if (user != null && await _userRoleManager.IsAdmin(user))
                return true;
            if (webpage == null)
                return false;
            if (!webpage.HasCustomPermissions || webpage.PermissionType != WebpagePermissionType.PasswordBased)
            {
                return true;
            }

            var cookieValue = cookies[GetCookieKey(webpage)];

            return cookieValue == webpage.PasswordAccessToken.ToString();
        }

        public void GiveAccessToPage(Webpage webpage, IResponseCookies cookies)
        {
            cookies.Append(GetCookieKey(webpage), webpage.PasswordAccessToken.ToString());
        }
    }
}