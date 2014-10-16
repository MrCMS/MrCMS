using System.Globalization;
using MrCMS.Website;

namespace MrCMS.Services.Resources
{
    public class GetCurrentUserCultureInfo : IGetCurrentUserCultureInfo
    {
        private readonly IGetUserCultureInfo _getUserCultureInfo;

        public GetCurrentUserCultureInfo(IGetUserCultureInfo getUserCultureInfo)
        {
            _getUserCultureInfo = getUserCultureInfo;
        }

        public CultureInfo Get()
        {
            return _getUserCultureInfo.Get(CurrentRequestData.CurrentUser);
        }

        public string GetInfoString()
        {
            return _getUserCultureInfo.GetInfoString(CurrentRequestData.CurrentUser);
        }
    }
}