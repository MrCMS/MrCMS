using System.Net;

namespace MrCMS.Website.PushNotifications
{
    public class WebPushResult
    {
        public HttpStatusCode? StatusCode { get; set; }
        public string Error => StatusCode.HasValue ? $"Failed with error code {StatusCode}" : null;
    }
}