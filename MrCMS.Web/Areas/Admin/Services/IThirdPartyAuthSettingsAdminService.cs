using System.Threading.Tasks;
using MrCMS.Settings;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IThirdPartyAuthSettingsAdminService
    {
        Task<ThirdPartyAuthSettings> GetSettingsAsync();
        ThirdPartyAuthSettings GetSettings();
        Task SaveSettingsAsync(ThirdPartyAuthSettings thirdPartyAuthSettings);
        void SaveSettings(ThirdPartyAuthSettings thirdPartyAuthSettings);
    }
}