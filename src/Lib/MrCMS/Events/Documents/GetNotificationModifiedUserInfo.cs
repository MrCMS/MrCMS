using System.Threading.Tasks;
using MrCMS.Helpers;
using MrCMS.Services;

namespace MrCMS.Events.Documents
{
    public class GetNotificationModifiedUserInfo : IGetNotificationModifiedUserInfo
    {
        private readonly IGetCurrentClaimsPrincipal _getCurrentClaimsPrincipal;

        public GetNotificationModifiedUserInfo(IGetCurrentClaimsPrincipal getCurrentClaimsPrincipal)
        {
            _getCurrentClaimsPrincipal = getCurrentClaimsPrincipal;
        }

        public async Task<string> GetInfo()
        {
            var user = await _getCurrentClaimsPrincipal.GetPrincipal();
            return user != null
                ? $" by <a href=\"/Admin/User/Edit/{user.GetUserId()}\">{user.GetFullName()}</a>"
                : string.Empty;
        }
    }
}