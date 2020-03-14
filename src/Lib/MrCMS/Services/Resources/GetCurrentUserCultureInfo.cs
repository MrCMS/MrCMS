using System.Globalization;
using System.Threading.Tasks;
using MrCMS.Website;

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
            return await _getUserCultureInfo.Get(_getCurrentUser.Get());
        }

        public async Task<string> GetInfoString()
        {
            return await _getUserCultureInfo.GetInfoString(_getCurrentUser.Get());
        }
    }
}