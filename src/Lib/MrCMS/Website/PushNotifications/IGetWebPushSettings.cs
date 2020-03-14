using System.Threading.Tasks;

namespace MrCMS.Website.PushNotifications
{
    public interface IGetWebPushSettings
    {
        Task<WebPushSettings> GetSettings();
    }
}