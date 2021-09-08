using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;

namespace MrCMS.Web.Apps.Core.Services
{
    public class PasswordProtectedPageChecker : IPasswordProtectedPageChecker
    {
        private readonly IGetCurrentUser _getCurrentUser;

        public PasswordProtectedPageChecker(IGetCurrentUser getCurrentUser)
        {
            _getCurrentUser = getCurrentUser;
        }

        private string GetCookieKey(Webpage webpage)
        {
            return $"MrCMS.PasswordProtectedPage.{webpage.Id}";
        }

        public async Task<bool> CanAccessPage(Webpage webpage, IRequestCookieCollection cookies)
        {
            var user = await _getCurrentUser.Get();
            if (user?.IsAdmin == true)
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