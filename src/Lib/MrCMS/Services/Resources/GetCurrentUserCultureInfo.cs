using System.Globalization;
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

        public CultureInfo Get()
        {
            return _getUserCultureInfo.Get(_getCurrentUser.Get());
        }

        public string GetInfoString()
        {
            return _getUserCultureInfo.GetInfoString(_getCurrentUser.Get());
        }
    }
}