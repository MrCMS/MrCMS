using System.Globalization;
using System.Threading.Tasks;

namespace MrCMS.Services.Resources
{
    public class GetCurrentUserCultureInfo : IGetCurrentUserCultureInfo
    {
        private readonly IGetUserCultureInfo _getUserCultureInfo;
        private readonly IGetCurrentUser _getCurrentUser;

        public GetCurrentUserCultureInfo(IGetUserCultureInfo getUserCultureInfo, IGetCurrentUser getCurrentUser)
        {
            _getUserCultureInfo = getUserCultureInfo;
            _getCurrentUser = getCurrentUser;
        }

        public async Task<CultureInfo> Get()
        {
            return _getUserCultureInfo.Get(await _getCurrentUser.Get());
        }

        public async Task<string> GetInfoString()
        {
            return _getUserCultureInfo.GetInfoString(await _getCurrentUser.Get());
        }
    }
}