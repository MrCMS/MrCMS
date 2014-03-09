using System.Threading.Tasks;
using MrCMS.Settings;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IThirdPartyAuthSettingsAdminService
    {
        ThirdPartyAuthSettings GetSettings();
        void SaveSettings(ThirdPartyAuthSettings thirdPartyAuthSettings);
    }
}