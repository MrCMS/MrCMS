using MrCMS.Settings;

namespace MrCMS.Web.Admin.Services
{
    public interface IThirdPartyAuthSettingsAdminService
    {
        ThirdPartyAuthSettings GetSettings();
        void SaveSettings(ThirdPartyAuthSettings thirdPartyAuthSettings);
    }
}