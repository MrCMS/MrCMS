using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Website.PushNotifications;

namespace MrCMS.Website.Controllers
{
    public class PushNotificationSubscriptionsController : MrCMSUIController
    {
        private readonly IPushNotificationSubscriptionManager _manager;

        public PushNotificationSubscriptionsController(IPushNotificationSubscriptionManager manager)
        {
            _manager = manager;
        }

        [HttpGet("sw.js")]
        public ContentResult ServiceWorkerJavaScript()
        {
            return Content(_manager.GetServiceWorkerJavaScript(), "text/javascript");
        }

        [HttpPost("push-notifications")]
        public async Task<IActionResult> Create([FromBody] PushNotificationSubscription subscription)
        {
            var result = await _manager.CreateSubscription(subscription);

            return GetResult(result);
        }

        [HttpDelete("push-notifications")]
        public async Task<IActionResult> Delete(string endpoint)
        {
            var result = await _manager.RemoveSubscription(endpoint);

            return GetResult(result);
        }

        private static IActionResult GetResult(WebPushResult result)
        {
            return new StatusCodeResult((int) result.StatusCode.GetValueOrDefault(HttpStatusCode.OK));
        }
    }
}