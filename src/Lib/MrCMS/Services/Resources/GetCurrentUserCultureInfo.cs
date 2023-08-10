using System.Globalization;
using System.Threading.Tasks;
using MrCMS.Helpers;

namespace MrCMS.Services.Resources
{
    public class GetCurrentUserCultureInfo : IGetCurrentUserCultureInfo
    {
        private readonly IGetUserCultureInfo _getUserCultureInfo;
        private readonly IGetCurrentClaimsPrincipal _getCurrentClaimsPrincipal;

        public GetCurrentUserCultureInfo(IGetUserCultureInfo getUserCultureInfo,
            IGetCurrentClaimsPrincipal getCurrentClaimsPrincipal)
        {
            _getUserCultureInfo = getUserCultureInfo;
            _getCurrentClaimsPrincipal = getCurrentClaimsPrincipal;
        }

        public async Task<CultureInfo> Get()
        {
            var user = await _getCurrentClaimsPrincipal.GetPrincipal();
            return _getUserCultureInfo.Get(user?.GetUserCulture());
        }

        public async Task<string> GetInfoString()
        {
            var user = await _getCurrentClaimsPrincipal.GetPrincipal();
            return _getUserCultureInfo.GetInfoString(user?.GetUserCulture());
        }
    }
}